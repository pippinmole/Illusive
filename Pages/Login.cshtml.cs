using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Illusive.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using reCAPTCHA.AspNetCore;

namespace Illusive.Pages {
    public class LoginModel : PageModel {
        
        [BindProperty] public LoginPost loginData { get; set; }

        private readonly ILogger<LoginModel> _logger;
        private readonly IRecaptchaService _recaptchaService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        public LoginModel(ILogger<LoginModel> logger, IRecaptchaService recaptchaService, SignInManager<ApplicationUser> signInManager) {
            _logger = logger;
            _recaptchaService = recaptchaService;
            _signInManager = signInManager;
        }
        
        public IActionResult OnGet() {
            try {
                if ( User.IsLoggedIn() ) {
                    return RedirectToPage("/Index");
                }
            } catch ( Exception ex ) {
                Console.Write(ex);
            }

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync(string returnUrl) {
            if ( !ModelState.IsValid )
                return Page();

            var result = await _recaptchaService.Validate(Request);
            if ( !result.success ) {
                ModelState.AddModelError("", "Recaptcha failed.");
                return Page();
            }
            
            var username = loginData.Username;
            var password = loginData.Password;
            var isPersistent = loginData.RememberMe;
            
            var signInResult = await _signInManager.PasswordSignInAsync(username, password, isPersistent, false);

            if ( signInResult.Succeeded ) {
                _logger.LogInformation($"{username} has logged in.");
            } else {
                _logger.LogWarning("Login attempt failed");
                ModelState.AddModelError("", "Login failed");
                return Page();
            }
            
            if ( !string.IsNullOrEmpty(returnUrl) ) {
                return LocalRedirect(returnUrl);
            }

            return RedirectToPage("/Index");
        }

        public class LoginPost {

            [Required] public string Username { get; set; }

            [Required, DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }

            public LoginPost() {
                
            }
        }
    }
}