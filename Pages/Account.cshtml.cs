using System;
using Illusive.Illusive.Database.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Illusive.Pages {
    public class AccountModel : PageModel {

        private readonly IDatabaseContext databaseContext;
        private readonly IAccountService accountService;

        public AccountModel(IDatabaseContext dbContext, IAccountService accService) {
            this.databaseContext = dbContext;
            this.accountService = accService ?? throw new NullReferenceException("AccountService needs to be setup!");
            
            Console.WriteLine($"AccountService: {this.accountService}");
        }
        
        public IActionResult OnGet() {
            if ( !this.User.Identity.IsAuthenticated )
                return this.RedirectToPage("/Index");

            // Get account data
            
            Console.WriteLine("Account OnGet");
            
            return this.Page();
        }

        public string GetData() {
            Console.WriteLine("Getting data for client");
            
            var account = this.accountService.GetAccount("email@email.com");
            if ( account != null ) {
                return $"Account ID: {account.Id} \n" +
                       $"Account Email: {account.Email} \n" +
                       $"Account Age: {account.Age}";
            }

            return "";
        }
    }
}