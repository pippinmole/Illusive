using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
using Illusive.Illusive.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Illusive.Pages {
    public class SignupModel : PageModel {

        private readonly IAccountService _accountService;
        
        [BindProperty] public SignupDataForm SignupData { get; set; }

        public SignupModel(IAccountService accountService) {
            this._accountService = accountService;
        }
        
        public void OnGet() {
            
        }

        public async Task<IActionResult> OnPost() {
            if ( !this.ModelState.IsValid )
                return this.Page();

            var username = this.SignupData.Username;
            var email = this.SignupData.Email;
            var password = this.SignupData.Password;

            if ( !email.IsEmail() ) {
                this.ModelState.AddModelError("", "The email you entered is not valid!");
                return this.Page();
            }

            var accountExists = this._accountService.AccountExists(
                account => account.AccountName == username || account.Email == email, out _);
            if ( accountExists ) {
                this.ModelState.AddModelError("", "An account with that username or email already exists!");
                return this.Page();
            }

            Console.WriteLine("Signed up with credentials: \n" +
                              $"username: {username} \n" +
                              $"email: {email} \n" +
                              $"password: {password} \n");

            var newAccount = new AccountData(
                Guid.NewGuid().ToString().Substring(0, 10), // TODO: Implement user id
                username,
                email,
                17,
                BCrypt.Net.BCrypt.HashPassword(password)
            );

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
                ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, username));
            identity.AddClaim(new Claim(ClaimTypes.Name, username));
            identity.AddClaim(new Claim(ClaimTypes.Email, email));
            identity.AddClaim(new Claim(ClaimTypes.PrimarySid, newAccount.Id));

            var principal = new ClaimsPrincipal(identity);
            await this.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties {IsPersistent = true});

            this._accountService.AddRecord(newAccount);

            Console.WriteLine($"Redirecting to /Account");

            return this.RedirectToPage("/Account");
        }
    }
}