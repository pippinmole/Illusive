using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Illusive.Pages {
    public class AccountModel : PageModel {
        public void OnGet() {
            Console.WriteLine("Account OnGet");
        }
    }
}