using System;
using System.Threading.Tasks;
using Illusive.Database;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Illusive.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class LogoutModel : PageModel {
        private readonly ILogger<LogoutModel> _logger;
        private readonly IAppUserManager _userManager;

        public LogoutModel(ILogger<LogoutModel> logger, IAppUserManager userManager) {
            this._logger = logger;
            this._userManager = userManager;
        }

        public async Task<IActionResult> OnGet() {
            if ( this.User.IsLoggedIn() ) {
                await this._userManager.SignOutAsync();
                this._logger.LogInformation($"{this.User.GetDisplayName()} has logged out.");
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}