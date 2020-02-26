using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySqlLogin.Data;
using MySqlLogin.Helpers;
using MySqlLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MySqlLogin.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext dataContext;
        
        private readonly IUserHelper userHelper;

        public AccountController(IUserHelper userHelper, DataContext dataContext)
        {
            this.userHelper = userHelper;
            this.dataContext = dataContext;
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

        public async Task<IActionResult> Logout()
        {
            await this.userHelper.LogoutAsync();
            return this.RedirectToAction("Index", "Home");
        }


    }
}
