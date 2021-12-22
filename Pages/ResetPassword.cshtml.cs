using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class ResetPasswordModel : PageModel {
        private readonly ILogger<ResetPasswordModel> _logger;
        private readonly IAppUserManager _userManager;
        
        [BindProperty] public ResetPasswordForm Form { get; set; }

        public ResetPasswordModel(ILogger<ResetPasswordModel> logger, IAppUserManager userManager) {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync(string email, string token) {
            if ( !ModelState.IsValid )
                return Page();

            var form = Form;

            var user = await _userManager.GetUserByEmailAsync(email);
            if ( user == null ) return Forbid();

            var result = await _userManager.ResetPasswordAsync(user, HttpUtility.UrlDecode(token), form.Password);
            if ( !result.Succeeded ) {
                foreach ( var error in result.Errors ) {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return Page();
            }

            return LocalRedirect("/Login");
        }
        
        public class ResetPasswordForm {
            [Required, DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string Token { get; set; }
        }
    }
}