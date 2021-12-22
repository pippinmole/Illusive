using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Illusive.Illusive.Core.User_Management.Extension_Methods;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Illusive.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Illusive.Illusive.Core.User_Management.Behaviour {
    public class AppUserManager : IAppUserManager {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AppUserManager> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;

        public AppUserManager(IConfiguration configuration, ILogger<AppUserManager> logger,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager, IMapper mapper) {
            _configuration = configuration;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;

            _roleManager.SetInitialRolesAsync(new List<string> {
                "Admin"
            });
        }

        public async Task<SafeApplicationUser> GetSafeUserAsync(ClaimsPrincipal principal) {
            var user = await GetUserAsync(principal);
            return _mapper.Map<SafeApplicationUser>(user);
        }

        public async Task<SafeApplicationUser> GetSafeUserByIdAsync(string id) {
            var user = await GetUserByIdAsync(id);
            return _mapper.Map<SafeApplicationUser>(user);
        }

        public async Task<SafeApplicationUser> GetSafeUserByIdAsync(Guid id) {
            var user = await GetUserByIdAsync(id);
            return _mapper.Map<SafeApplicationUser>(user);
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal) {
            return await _userManager.GetUserAsync(principal);
        }
        
        public async Task<ApplicationUser> GetUserByIdAsync(string id) {
            if ( Guid.TryParse(id, out var guid) )
                return await _userManager.FindByIdAsync(guid.ToString());
            else {
                _logger.LogWarning("User Guid given is not in the correct format!");
                return null;
            }
        }
        
        public async Task<ApplicationUser> GetUserByIdAsync(Guid id) {
            return await _userManager.FindByIdAsync(id.ToString());
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user) {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> RemoveUserAsync(ApplicationUser user) {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user) {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email) {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task SignOutAsync() {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string password) {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role) {
            return await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<bool> IsUserInRole(ApplicationUser user, string role) {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser newAccount, string password) {
            newAccount.GenerateApiKey(_configuration);
            return await _userManager.CreateAsync(newAccount, password);
        }

        public async Task SignInAsync(ApplicationUser newAccount, bool isPersistent) {
            await _signInManager.SignInAsync(newAccount, isPersistent);
        }
    }
}