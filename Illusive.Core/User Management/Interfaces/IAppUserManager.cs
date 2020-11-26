using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Illusive.Illusive.Core.User_Management.Interfaces {
    public interface IAppUserManager {
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<ApplicationUser> GetUserByIdAsync(Guid id);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> RemoveUserAsync(ApplicationUser user);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task SignOutAsync();
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string password);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<bool> IsUserInRole(ApplicationUser user, string role);
    }
}