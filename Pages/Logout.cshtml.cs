using System.Threading.Tasks;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Illusive.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class LogoutModel : PageModel {
        private readonly ILogger<LogoutModel> _logger;
        private readonly IAppUserManager _userManager;

        public LogoutModel(ILogger<LogoutModel> logger, IAppUserManager userManager) {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet() {
            if ( User.IsLoggedIn() ) {
                await _userManager.SignOutAsync();
                _logger.LogInformation($"{User.GetDisplayName()} has logged out.");
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}