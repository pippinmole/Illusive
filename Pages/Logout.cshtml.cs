using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Illusive.Authentication.Utility;
using Illusive.Illusive.Database.Interfaces;
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
                var user = this._accountService.GetAccountWhere(x => x.Id == this.User.GetUniqueId());
                
                if ( this.User.IsLoggedIn() ) {
                    await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    
                    this._logger.LogInformation($"{user.AccountName} has logged out.");
                }
            } catch ( Exception e ) {
                this._logger.LogError($"Error logging out {this.User.GetUniqueId()}: {e}");
            }
            
            return this.RedirectToPage("/Index");
        }
    }
}