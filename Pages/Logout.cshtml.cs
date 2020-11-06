using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Illusive.Authentication.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class LogoutModel : PageModel {

        public async Task<RedirectToPageResult> OnGet() {
            Console.WriteLine($"Logging player out: IsAuth: {this.User.IsLoggedIn()}");
            
            try {
                if ( this.User.IsLoggedIn() ) {
                    await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }
            } catch ( Exception ex ) {
                Console.Write(ex);
            }
            
            return this.RedirectToPage("/Index");
        }
    }
}