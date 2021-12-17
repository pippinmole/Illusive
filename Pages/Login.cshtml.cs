using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Illusive.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using reCAPTCHA.AspNetCore;

namespace Illusive.Pages {
    public class LoginModel : PageModel {
        
        [BindProperty] public LoginPost loginData { get; set; }

        private readonly ILogger<LoginModel> _logger;
        private readonly IRecaptchaService _recaptchaService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        public LoginModel(ILogger<LoginModel> logger, IRecaptchaService recaptchaService, SignInManager<ApplicationUser> signInManager) {
            this._logger = logger;
            this._recaptchaService = recaptchaService;
            this._signInManager = signInManager;
        }
        
        public IActionResult OnGet() {
            try {
                if ( this.User.IsLoggedIn() ) {
                    return this.RedirectToPage("/Index");
                }
            } catch ( Exception ex ) {
                Console.Write(ex);
            }

            return this.Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string returnUrl) {
            if ( !this.ModelState.IsValid )
                return this.Page();

            var result = await this._recaptchaService.Validate(this.Request);
            if ( !result.success ) {
                this.ModelState.AddModelError("", "Recaptcha failed.");
                return this.Page();
            }
            
            var username = this.loginData.Username;
            var password = this.loginData.Password;
            var isPersistent = this.loginData.RememberMe;
            
            var signInResult = await this._signInManager.PasswordSignInAsync(username, password, isPersistent, false);

            if ( signInResult.Succeeded ) {
                this._logger.LogInformation($"{username} has logged in.");
            } else {
                this._logger.LogWarning("Login attempt failed");
                this.ModelState.AddModelError("", "Login failed");
                return this.Page();
            }
            
            if ( !string.IsNullOrEmpty(returnUrl) ) {
                return this.LocalRedirect(returnUrl);
            }

            return this.RedirectToPage("/Index");
        }

        public class LoginPost {

            [Required] public string Username { get; set; }

            [Required, DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }

            public LoginPost() {
                
            }
        }
    }
}