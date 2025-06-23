using FlyTickets2025.web.Data.Entities;
using FlyTickets2025.web.Models;
using Microsoft.AspNetCore.Identity;

namespace FlyTickets2025.web.Helpers
{
    public interface IApplicationUserHelper
    {
        Task<ApplicationUser?> GetUserByEmailasync(string email);
        Task<IdentityResult> AddUserAsync(ApplicationUser user, string password);

        Task<SignInResult> LoginAsync(LoginViewModel login);
        Task LogoutAsync();

        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string oldPassword, string newPassword);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);

        Task CheckRoleAsync(string roleName);
        Task AddUserToRoleAsync(ApplicationUser user, string roleName);
        Task<bool> IsUserInRoleAsync(ApplicationUser user, string roleName);
    }
}
