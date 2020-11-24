using System;
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
            this._logger = logger;
            this._userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync(string email, string token) {
            if ( !ModelState.IsValid )
                return this.Page();

            var form = this.Form;

            var user = await this._userManager.GetUserByEmailAsync(email);
            if ( user == null ) return this.Forbid();

            var result = await this._userManager.ResetPasswordAsync(user, HttpUtility.UrlDecode(token), form.Password);
            if ( !result.Succeeded ) {
                foreach ( var error in result.Errors ) {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }

                return this.Page();
            }

            return this.LocalRedirect("/Login");
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