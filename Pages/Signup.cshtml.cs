using System.Threading.Tasks;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Illusive.Models;
using Illusive.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class SignupModel : PageModel {
        private readonly ILogger<SignupModel> _logger;
        private readonly IAppUserManager _userManager;

        [BindProperty] public SignupDataForm SignupData { get; set; }

        public SignupModel(ILogger<SignupModel> logger, IAppUserManager userManager) {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPost() {
            if ( !ModelState.IsValid )
                return Page();

            var username = SignupData.Username.Trim();
            var email = SignupData.Email.Trim();
            var password = SignupData.Password.Trim();

            if ( !email.IsEmail() ) {
                ModelState.AddModelError("", "The email you entered is not valid!");
                return Page();
            }

            var newAccount = new ApplicationUser(username, email);
            
            var result = await _userManager.CreateAsync(newAccount, password);
            if ( !result.Succeeded ) {
                foreach ( var error in result.Errors ) {
                    ModelState.AddModelError(error.Code, error.Description);
                    _logger.LogWarning($"Error creating user account: {error.Description}");
                }

                return Page();
            }

            _logger.LogInformation($"Successfully created account with username {username}");

            await _userManager.SignInAsync(newAccount, true);

            return RedirectToPage("/Account");
        }
    }
}