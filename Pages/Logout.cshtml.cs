using System;
using System.Threading.Tasks;
using Illusive.Database;
using Illusive.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class LogoutModel : PageModel {
        private readonly ILogger<LogoutModel> _logger;
        private readonly IAccountService _accountService;

        public LogoutModel(ILogger<LogoutModel> logger, IAccountService accountService) {
            this._logger = logger;
            this._accountService = accountService;
        }
        
        public async Task<RedirectToPageResult> OnGet() {
            try {
                if ( this.User.IsLoggedIn() ) {
                    await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    
                    this._logger.LogInformation($"{this.User.GetDisplayName()} has logged out.");
                }
            } catch ( Exception e ) {
                this._logger.LogError($"Error logging out {this.User.GetUniqueId()}: {e}");
            }
            
            return this.RedirectToPage("/Index");
        }
    }
}