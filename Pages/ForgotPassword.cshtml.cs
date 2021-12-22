using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web;
using Illusive.Illusive.Core.Mail.Interfaces;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class ForgotPasswordModel : PageModel {
        private readonly ILogger<ForgotPasswordModel> _logger;
        private readonly IMailSender _mailSender;
        private readonly IAppUserManager _userManager;

        [BindProperty] public ForgotPasswordForm Form { get; set; }

        public ForgotPasswordModel(ILogger<ForgotPasswordModel> logger, IMailSender mailSender, IAppUserManager userManager) {
            _logger = logger;
            _mailSender = mailSender;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync() {
            var form = Form;

            if ( !string.IsNullOrEmpty(form.Email) ) {
                var account = await _userManager.GetUserByEmailAsync(form.Email);
                if ( account == null ) {
                    _logger.LogWarning($"An account with email {form.Email} does not exist!");
                    return Page();
                }

                var token = HttpUtility.UrlEncode(await _userManager.GeneratePasswordResetTokenAsync(account));
                var callback = Url.Page("ResetPassword", new {
                    token,
                    email = form.Email
                });

                await _mailSender.SendEmailAsync(
                    new List<string> {
                        form.Email
                    },
                    "Account Password Recovery",
                    $"{Request.Scheme}://{Request.Host}{Request.PathBase}{callback}", 
                    null);

                _logger.LogInformation($"{form.Email} is trying to reset their password.");

                return LocalRedirect("/");
            }

            return Page();
        }

        public IActionResult OnGet(string email, string token) {
            return Page();
        }
        
        public class ForgotPasswordForm {
            [Required]
            public string Email { get; set; }
        }
    }
}