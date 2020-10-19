using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Illusive.Database.Interfaces;
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

        public async Task<IActionResult> OnPostAsync() {
            if ( this.ModelState.IsValid ) {
                
                Console.WriteLine($"Login request from {this.loginData.Username}");

                var username = this.loginData.Username;
                var password = this.loginData.Password;

                this._logger.LogWarning($"Hashed '{password}' to {BCrypt.Net.BCrypt.HashPassword(password)}");

                var isValid = this._accountService.AccountExists(x => x.AccountName == username, out var account);
                var passwordMatches = account?.VerifyPasswordHashEquals(password) == true;
                
                this._logger.LogWarning($"Password matches: {passwordMatches}");
                
                isValid &= passwordMatches;
                if ( !isValid ) {
                    this.ModelState.AddModelError("", "username or password is invalid");
                    return this.Page();
                }
                
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
                    ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, this.loginData.Username));
                identity.AddClaim(new Claim(ClaimTypes.Name, this.loginData.Username));
                identity.AddClaim(new Claim(ClaimTypes.Email, account.Email));

                var principal = new ClaimsPrincipal(identity);
                await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties {IsPersistent = this.loginData.RememberMe});

                Console.WriteLine($"Redirecting to /Index");
                
                return this.RedirectToPage("/Index");
            } else {
                this.ModelState.AddModelError("", "username or password is blank");
                return this.Page();
            }
        }

        public class LoginPost {

            [Required] public string Username { get; set; }

            [Required, DataType(DataType.Password)]
            public string Password { get; set; }
            public bool RememberMe { get; set; }
        }
    }
}