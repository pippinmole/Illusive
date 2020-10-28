using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class LoginModel : PageModel {
        
        [BindProperty] public LoginPost loginData { get; set; }

        private readonly ILogger<LoginModel> _logger;
        private readonly IAccountService _accountService;
        
        public LoginModel(IAccountService accountService, ILogger<LoginModel> logger) {
            this._accountService = accountService;
            this._logger = logger;
        }
        
        public IActionResult OnGet() {
            Console.WriteLine("OnLoginPageGet");
            try {
                if ( this.User.Identity.IsAuthenticated ) {
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

            var username = this.loginData.Username;
            var password = this.loginData.Password;
            var rememberMe = this.loginData.RememberMe;

            var accountExists = this._accountService.AccountExists(x => x.AccountName == username, out var account);
            if ( !accountExists || !account.VerifyPasswordHashEquals(password)) {
                this.ModelState.AddModelError("", "username or password is invalid");
                return this.Page();
            }
                
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
                ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.Name, username));
            identity.AddClaim(new Claim(ClaimTypes.Email, account?.Email));
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, account?.Id));

            var principal = new ClaimsPrincipal(identity);
            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties {
                    IsPersistent = rememberMe
                });

            this._logger.LogDebug($"Redirecting logged-in user {account?.AccountName} to /Index");

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
        }
    }
}