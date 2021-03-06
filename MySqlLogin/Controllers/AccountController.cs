﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MySqlLogin.Data;
using MySqlLogin.Helpers;
using MySqlLogin.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MySqlLogin.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext dataContext;
        
        private readonly IUserHelper userHelper;
        private readonly IConfiguration configuration;

        public AccountController(IUserHelper userHelper, DataContext dataContext, IConfiguration configuration)
        {
            this.userHelper = userHelper;
            this.dataContext = dataContext;
            this.configuration = configuration;
        }

        public IActionResult Login()
        {
            if (this.User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Home");
            }

            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)

        {
            if (this.ModelState.IsValid)
            {
                var result = await this.userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return this.Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Usuario y/o contraseña invalidos.");
            return this.View(model);
        }

        public IActionResult Register()
        {            
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userHelper.GetUserByEmailAsync(model.Username);

                //NO EXISTE EL USUARIO EN LA BASE DE DATOS
                if (user == null)
                {
                    try
                    {
                        //GUARDO EL USUARIO EN LA TABLA ENTIDAD DE ARMADILLO BASE
                        var entidadArmadillo = new EntidadArmadillo
                        {
                           RUT_ENTIDAD = model.RutEntidad,
                           CLAVE_ENTIDAD = GetSHA1(model.ClaveEntidad),
                           USUARIO_ENTIDAD = null,
                           ENT_SALT = "123456",
                           ENT_ENC = "SHA1",
                           ENT_ACTIVANDO = false,
                           ENT_RESTABLECIMIENTO = false,
                           ENT_EMAIL = model.Username,
                           ENT_TIPO = 2,
                           ENT_REFERER = null,
                        };

                        dataContext.ENTIDAD.Add(entidadArmadillo);
                        await dataContext.SaveChangesAsync();
                     
                        //ARMO OTRO OBJETO DIFERENTE PARA GUARDAR EN LA TABLA ASPNET USERS
                        user = new Entidad
                        {
                            RutEntidad = model.RutEntidad,
                            UserName = model.Username,
                            EntTipo = model.EntTipo,
                            Email = model.Username,
                        };

                        var result = await this.userHelper.AddUserAsync(user, model.ClaveEntidad);
                        if (result != IdentityResult.Success)
                        {
                            this.ModelState.AddModelError(string.Empty, "El usuario no pudo ser creado.");
                            return this.View(model);
                        }

                        this.ModelState.AddModelError(string.Empty, "Usuario creado con exito.");

                        return this.View(model);
                    }
                    catch (Exception Error)
                    {
                        this.ModelState.AddModelError(string.Empty, "No se pudo registrar al usuario, intente más tarde.");                       
                    }

                }

                //USUARIO YA EXISTE EN LA BASE DE DATOS
                this.ModelState.AddModelError(string.Empty, "El usuario que se intenta registrar ya esta creado en el sistema.");
            }

            return this.View(model);
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "El email no coincide con el usuario registrado.");
                    return this.View(model);
                }

                //var myToken = await this.userHelper.GeneratePasswordResetTokenAsync(user);
                //var link = this.Url.Action("ResetPassword", "Account", new { token = myToken }, protocol: HttpContext.Request.Scheme);
                //var mailSender = new MailHelper(configuration);
                //mailSender.SendMail(model.Email, "Resetear Contraseña", $"<h1>Recuperar Contraseña</h1>" +
                //    $"Para resetear el password, haga click aquí:</br></br> " +
                //    $"<a href = \"{link}\">Recuperar Contraseña</a>");
                //this.ViewBag.Message = "Las instrucciones para recuperar la contraseña fueron enviadas a su correo.";
                //return this.View();

            }

            return this.View(model);
        }

        public IActionResult NotAuthorized()
        {
            return this.View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            configuration["Tokens:Issuer"],
                            configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }

     
        public async Task<IActionResult> Logout()
        {
            await this.userHelper.LogoutAsync();
            return this.RedirectToAction("Index", "Home");
        }

        public static string GetSHA1(string str)
        {
            SHA1 sha1 = SHA1Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha1.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);

            return sb.ToString();
        }

    }
}
