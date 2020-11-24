using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Illusive.Illusive.Core.User_Management.Behaviour {
    public class AppUserManager : IAppUserManager {
        
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AppUserManager(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal) {
            return await this._userManager.GetUserAsync(principal);
        }
        
        public async Task<ApplicationUser> GetUserByIdAsync(string id) {
            return await this._userManager.FindByIdAsync(id);
        }
        
        public async Task<ApplicationUser> GetUserByIdAsync(Guid id) {
            return await this._userManager.FindByIdAsync(id.ToString());
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user) {
            return await this._userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> RemoveUserAsync(ApplicationUser user) {
            return await this._userManager.DeleteAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user) {
            return await this._userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email) {
            return await this._userManager.FindByEmailAsync(email);
        }

        public async Task SignOutAsync() {
            await this._signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string password) {
            return await this._userManager.ResetPasswordAsync(user, token, password);
        }
    }
}