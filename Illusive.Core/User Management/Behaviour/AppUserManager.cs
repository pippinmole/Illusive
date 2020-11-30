﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Illusive.Core.User_Management.Extension_Methods;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Illusive.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Illusive.Illusive.Core.User_Management.Behaviour {
    public class AppUserManager : IAppUserManager {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AppUserManager(IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager) {
            this._configuration = configuration;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            
            this._roleManager.SetInitialRolesAsync(new List<string> {
                "Admin"
            });
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

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role) {
            return await this._userManager.AddToRoleAsync(user, role);
        }

        public async Task<bool> IsUserInRole(ApplicationUser user, string role) {
            return await this._userManager.IsInRoleAsync(user, role);
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser newAccount, string password) {
            newAccount.GenerateApiKey(this._configuration);
            return await this._userManager.CreateAsync(newAccount, password);
        }

        public async Task SignInAsync(ApplicationUser newAccount, bool isPersistent) {
            await this._signInManager.SignInAsync(newAccount, isPersistent);
        }
    }
}