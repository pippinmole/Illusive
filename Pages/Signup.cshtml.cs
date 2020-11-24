using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Database;
using Illusive.Models;
using Illusive.Models.Extensions;
using Illusive.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class SignupModel : PageModel {
        private readonly ILogger<SignupModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        [BindProperty] public SignupDataForm SignupData { get; set; }

        public SignupModel(ILogger<SignupModel> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
            this._logger = logger;
            this._userManager = userManager;
            this._signInManager = signInManager;
        }
        
        public void OnGet() {
            
        }

        public async Task<IActionResult> OnPost() {
            if ( !this.ModelState.IsValid )
                return this.Page();

            var username = this.SignupData.Username.Trim();
            var email = this.SignupData.Email.Trim();
            var password = this.SignupData.Password.Trim();

            if ( !email.IsEmail() ) {
                this.ModelState.AddModelError("", "The email you entered is not valid!");
                return this.Page();
            }

            var newAccount = new ApplicationUser(
                userName: username,
                email: email
            );

            var result = await this._userManager.CreateAsync(newAccount, password);
            if ( !result.Succeeded ) {
                foreach ( var error in result.Errors ) {
                    this.ModelState.AddModelError(error.Code, error.Description);
                    this._logger.LogWarning($"Error creating user account: {error.Description}");
                }

                return this.Page();
            } else {
                this._logger.LogInformation($"Successfully created account with username {username}");

                await this._signInManager.SignInAsync(newAccount, true);
            }

            return this.RedirectToPage("/Account");
        }
    }
}