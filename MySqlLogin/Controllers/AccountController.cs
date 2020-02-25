using Microsoft.AspNetCore.Mvc;
using MySqlLogin.Helpers;
using MySqlLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySqlLogin.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper userHelper;
        public AccountController(IUserHelper userHelper)
        {
            this.userHelper = userHelper;         
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
                var user = await this.userHelper.GetUserByEmailAsync(model.UsuarioEntidad);

                //NO EXISTE EL USUARIO EN LA BASE DE DATOS
                if (user == null)
                {
                    try
                    {                     

                        user = new Entidad
                        {
                            RutEntidad = model.RutEntidad,
                            UsuarioEntidad = model.UsuarioEntidad,
                            EntEmail = model.EntEmail,
                            EntTipo = model.EntTipo,



                            //Nombre = model.FirstName,
                            //Apellido = model.LastName,
                            //Email = model.Username,
                            //UserName = model.Username,
                            //Address = model.Address,
                            //PhoneNumber = model.PhoneNumber,
                            //CityId = model.CityId,
                            //City = city
                        };

                    //    var result = await this.userHelper.AddUserAsync(user, model.Password);
                    //    if (result != IdentityResult.Success)
                    //    {
                    //        this.ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                    //        return this.View(model);
                    //    }

                        //    var myToken = await this.userHelper.GenerateEmailConfirmationTokenAsync(user);
                        //    var tokenLink = this.Url.Action("ConfirmEmail", "Account", new
                        //    {
                        //        userid = user.Id,
                        //        token = myToken
                        //    }, protocol: HttpContext.Request.Scheme);


                        //    this.mailHelper.SendMail(model.Username, "Email de confirmación", $"<h1>Email de confirmación</h1>" +
                        //       $"Para habilitar el usuario, " + $"haga click en este link: </br></br><a href = \"{tokenLink}\">Confirmar Email</a>");

                        //    this.ViewBag.Message = "Las instrucciones para registrar el usuario se enviaron a su correo.";

                        //    return this.View(model);
                    }
                    catch (Exception Error)
                    {
                        this.ModelState.AddModelError(string.Empty, "No se pudo registrar al usuario, intente más tarde.");

                        //this.ModelState.AddModelError(string.Empty, Error.Message);
                    }

                }

                //USUARIO YA EXISTE EN LA BASE DE DATOS
                this.ModelState.AddModelError(string.Empty, "El usuario que se intenta registrar ya esta creado en el sistema.");
            }

            return this.View(model);
        }


        public async Task<IActionResult> Logout()
        {
            await this.userHelper.LogoutAsync();
            return this.RedirectToAction("Index", "Home");
        }


    }
}
