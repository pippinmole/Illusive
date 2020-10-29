using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DnsClient.Internal;
using Illusive.Illusive.Database.Extension_Methods;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
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
            if ( !this.User.Identity.IsAuthenticated )
                return this.RedirectToPage("/Index");
            
            return this.Page();
        }

        public AccountData GetData() {
            var email = this.User.FindFirst(ClaimTypes.Email)?.Value;
            return this._accountService.GetAccount(email);
        }
    }
}