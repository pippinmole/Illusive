using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Illusive.Pages {
    public class AccountModel : PageModel {
        public IActionResult OnGet() {
            if ( !this.User.Identity.IsAuthenticated )
                return this.RedirectToPage("/Index");

            // Get account data
            
            Console.WriteLine("Account OnGet");
            
            return this.Page();
        }
    }
}