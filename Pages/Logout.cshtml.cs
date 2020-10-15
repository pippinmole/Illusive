using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class LogoutModel : PageModel {

        public async Task<RedirectToPageResult> OnGet() {
            Console.WriteLine($"Logging player out: IsAuth: {this.User.Identity.IsAuthenticated}");
            
            try {
                if ( this.User.Identity.IsAuthenticated ) {
                    await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            } catch ( Exception ex ) {
                Console.Write(ex);
            }
            
            return this.RedirectToPage("/Index");
        }
    }
}