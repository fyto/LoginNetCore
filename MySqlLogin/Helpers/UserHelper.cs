using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MySqlLogin.Models;

namespace MySqlLogin.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<Entidad> userManager;
        //private readonly SignInManager<Entidad> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;


        public UserHelper(UserManager<Entidad> userManager,/* SignInManager<User> signInManager,*/ RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            //this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public async Task<IdentityResult> AddUserAsync(Entidad entidad, string password)
        {
            return await this.userManager.CreateAsync(entidad, password);
        }

        public async Task AddUserToRoleAsync(Entidad entidad, string roleName)
        {
            await this.userManager.AddToRoleAsync(entidad, roleName);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await this.roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await this.roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task<Entidad> GetUserByEmailAsync(string email)
        {
            return await this.userManager.FindByEmailAsync(email);
        }

        public async Task<bool> IsUserInRoleAsync(Entidad entidad, string roleName)
        {
            return await this.userManager.IsInRoleAsync(entidad, roleName);
        }
    }
}
