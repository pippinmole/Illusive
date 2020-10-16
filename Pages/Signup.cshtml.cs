using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Illusive.Pages {
    public class SignupModel : PageModel {

        [BindProperty] public SignupDataForm SignupData { get; set; }
        
        public void OnGet() {
            
        }

        public async Task<IActionResult> OnPost() {
            if ( this.ModelState.IsValid ) {

                var username = this.SignupData.Username;
                var email = this.SignupData.Email;
                var password = this.SignupData.Password;

                Console.WriteLine("Logged in with credentials: \n" +
                                  $"username: {username} \n" +
                                  $"email: {email} \n" +
                                  $"password: {password} \n");

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
                    ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, username));
                identity.AddClaim(new Claim(ClaimTypes.Name, username));
                
                var principal = new ClaimsPrincipal(identity);
                await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                    new AuthenticationProperties {IsPersistent = true});

                Console.WriteLine($"Redirecting to /Account");
                
                return this.RedirectToPage("/Account");
            } else {
                this.ModelState.AddModelError("", "username or password is blank");
                return this.Page();
            }
        }

        public class SignupDataForm {
            
            [Required]
            public string Username { get; set; }
            
            [Required, DataType(DataType.EmailAddress)]
            public string Email { get; set; }
            
            [Required, DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}