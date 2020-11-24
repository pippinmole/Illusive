using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web;
using Illusive.Database;
using Illusive.Illusive.Core.Mail.Interfaces;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Illusive.Models;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Westwind.AspnetCore.Markdown.Utilities;

namespace Illusive.Pages {
    public class ForgotPasswordModel : PageModel {
        private readonly ILogger<ForgotPasswordModel> _logger;
        private readonly IMailSender _mailSender;
        private readonly IAppUserManager _userManager;

        [BindProperty] public ForgotPasswordForm Form { get; set; }

        public ForgotPasswordModel(ILogger<ForgotPasswordModel> logger, IMailSender mailSender, IAppUserManager userManager) {
            this._logger = logger;
            this._mailSender = mailSender;
            this._userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync() {
            var form = this.Form;

            if ( !string.IsNullOrEmpty(form.Email) ) {
                var account = await this._userManager.GetUserByEmailAsync(form.Email);
                if ( account == null ) {
                    this._logger.LogWarning($"An account with email {form.Email} does not exist!");
                    return this.Page();
                }

                var token = HttpUtility.UrlEncode(await this._userManager.GeneratePasswordResetTokenAsync(account));
                var callback = this.Url.Page("ResetPassword", new {
                    token,
                    email = form.Email
                });

                await this._mailSender.SendEmailAsync(new List<string> {
                        form.Email
                    }, "Account Password Recovery",
                    $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}{callback}", null);

                this._logger.LogInformation($"{form.Email} is trying to reset their password.");

                return this.LocalRedirect("/");
            }

            return this.Page();
        }

        public IActionResult OnGet(string email, string token) {
            return this.Page();
        }
        
        public class ForgotPasswordForm {
            [Required]
            public string Email { get; set; }
        }
    }
}