using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Database;
using Illusive.Models;
using Illusive.Models.Extensions;
using Illusive.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class SignupModel : PageModel {
        private readonly ILogger<SignupModel> _logger;
        private readonly IAccountService _accountService;
        
        [BindProperty] public SignupDataForm SignupData { get; set; }

        public SignupModel(ILogger<SignupModel> logger, IAccountService accountService) {
            this._logger = logger;
            this._accountService = accountService;
        }
        
        public void OnGet() {
            
        }

        public async Task<IActionResult> OnPost() {
            if ( !this.ModelState.IsValid )
                return this.Page();

            var username = this.SignupData.Username.Trim();
            var email = this.SignupData.Email.Trim();
            var password = this.SignupData.Password.Trim();

            if ( !email.IsEmail() ) {
                this.ModelState.AddModelError("", "The email you entered is not valid!");
                return this.Page();
            }

            var accountExists = this._accountService.AccountExistsWhere(
                account => account.AccountName == username || account.Email == email, out _);
            if ( accountExists ) {
                this.ModelState.AddModelError("", "An account with that username or email already exists!");
                return this.Page();
            }

            this._logger.LogInformation($"{username} has signed up with email {email}");

            var newAccount = new AccountData(
                Guid.NewGuid().ToString().Substring(0, 10),
                username,
                email,
                17,
                BCrypt.Net.BCrypt.HashPassword(password),
                false
            );

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
                ClaimTypes.Role);
            identity.AddClaimsForAccount(newAccount);

            var principal = new ClaimsPrincipal(identity);
            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties {IsPersistent = true});

            this._logger.LogInformation($"{username} has logged in.");
            
            this._accountService.AddRecord(newAccount);

            return this.RedirectToPage("/Account");
        }
    }
}