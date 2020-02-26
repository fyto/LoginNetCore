using Microsoft.AspNetCore.Identity;
using MySqlLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySqlLogin.Helpers
{
    public interface IUserHelper
    {
        Task<Entidad> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(Entidad entidad, string password);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(Entidad entidad, string roleName);

        Task<bool> IsUserInRoleAsync(Entidad entidad, string roleName);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task<SignInResult> ValidatePasswordAsync(Entidad entidad, string password);

        Task LogoutAsync();

    }
}
