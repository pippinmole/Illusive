using System;
using DnsClient.Internal;
using Illusive.Illusive.Database.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class AccountModel : PageModel {
        
        private readonly ILogger<AccountModel> _logger;
        private readonly IAccountService _accountService;

        public AccountModel(ILogger<AccountModel> logger, IAccountService accService) {
            this._logger = logger;
            this._accountService = accService ?? throw new NullReferenceException("AccountService needs to be setup!");
            
            Console.WriteLine($"AccountService: {this._accountService}");
        }
        
        public IActionResult OnGet() {
            this._logger.LogWarning($"OnGet /Account IsAuth{this.}");
            if ( !this.User.Identity.IsAuthenticated )
                return this.RedirectToPage("/Index");
            
            return this.Page();
        }

        public string GetData() {
            this._logger.LogWarning($"Getting account details for email@email.com");
            
            var account = this._accountService.GetAccount("email@email.com");
            if ( account != null ) {
                return $"Account ID: {account.Id} \n" +
                       $"Account Email: {account.Email} \n" +
                       $"Account Age: {account.Age}";
            }

            return "";
        }
    }
}