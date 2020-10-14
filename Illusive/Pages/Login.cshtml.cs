using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Illusive.Pages {
    public class LoginModel : PageModel {

        [BindProperty] public LoginPost loginData { get; set; }

        public IActionResult OnGet() {
            
            Console.WriteLine($"OnGet IsAuth: {this.User.Identity.IsAuthenticated}");
            
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
                
                var isValid = (this.loginData.Username == "username" && this.loginData.Password == "password");
                if ( !isValid ) {
                    this.ModelState.AddModelError("", "username or password is invalid");
                    return this.Page();
                }

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
                    ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, this.loginData.Username));
                identity.AddClaim(new Claim(ClaimTypes.Name, this.loginData.Username));
                
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