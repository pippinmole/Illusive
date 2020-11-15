using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Attributes;
using Illusive.Database;
using Illusive.Models.Extensions;
using Illusive.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using reCAPTCHA.AspNetCore;
using reCAPTCHA.AspNetCore.Attributes;
using reCAPTCHA.AspNetCore.Templates;

namespace Illusive.Pages {
    public class LoginModel : PageModel {
        
        [BindProperty] public LoginPost loginData { get; set; }

        private readonly ILogger<LoginModel> _logger;
        private readonly IRecaptchaService _recaptchaService;
        private readonly IConfiguration _configuration;
        private readonly IAccountService _accountService;
        
        public LoginModel(IConfiguration configuration, IAccountService accountService, ILogger<LoginModel> logger, IRecaptchaService recaptchaService) {
            this._configuration = configuration;
            this._accountService = accountService;
            this._logger = logger;
            this._recaptchaService = recaptchaService;
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
            var rememberMe = this.loginData.RememberMe;

            var accountExists = this._accountService.AccountExistsWhere(x => x.AccountName == username, out var account);
            if ( !accountExists || !account.VerifyPasswordHashEquals(password)) {
                this.ModelState.AddModelError("", "username or password is invalid");
                return this.Page();
            }
                
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
                ClaimTypes.Role);
            identity.AddClaimsForAccount(account);

            var principal = new ClaimsPrincipal(identity);
            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = rememberMe });
            
            this._logger.LogInformation($"{account?.AccountName} has logged in.");
            
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