#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Illusive.Authentication.Utility;
using Illusive.Illusive.Cdn.Interfaces;
using Illusive.Illusive.Data;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Illusive.Pages {
    public class AccountModel : PageModel {

        private readonly ILogger<AccountModel> _logger;
        private readonly IAccountService _accountService;
        private readonly IContentService _contentService;
        
        [BindProperty] public AccountChangeData AccountUpdate { get; set; }
        
        public AccountModel(ILogger<AccountModel> logger, IAccountService accService, IContentService contentService) {
            this._logger = logger;
            this._accountService = accService ?? throw new NullReferenceException("AccountService needs to be setup!");
            this._contentService = contentService;
        }

        public IActionResult OnGet(string? accountId) {
            this._logger.LogWarning($"Was Account ID supplied? {accountId != null}");

            var accountSupplied = accountId != null;
            if ( accountSupplied ) {
                var account = this._accountService.GetAccountWhere(x => x.Id == accountId);
                if ( account == null )
                    return this.NotFound();
            }
            
            if ( !this.User.IsLoggedIn() )
                return this.RedirectToPage("/Index");

            return this.Page();
        }
        
        public async Task<IActionResult> OnPostUploadAsync() {
            if ( !this.ModelState.IsValid )
                return this.Page();

            var accountId = this.User.GetUniqueId();
            var accountUpdate = this.AccountUpdate ?? throw new NullReferenceException("Account Change data shouldn't be null!");

            var profileBio = accountUpdate.Bio;
            if ( !string.IsNullOrEmpty(profileBio) ) {
                var update = Builders<AccountData>.Update.Set(x => x.Bio, profileBio);
                this._accountService.UpdateAccount(accountId, update);
            }
            
            var profilePic = accountUpdate.ProfilePicture;
            if ( profilePic != null && profilePic.Length > 0 ) {
                var filePath = Path.GetTempFileName();

                await using var stream = System.IO.File.Create(filePath, (int) profilePic.Length);
                await profilePic.CopyToAsync(stream);

                var path = await this._contentService.UploadFileAsync(Path.GetFileName(profilePic.FileName), stream);

                var update = Builders<AccountData>.Update.Set(x => x.ProfilePicture, path);
                this._accountService.UpdateAccount(accountId, update);
            }

            return this.Page();
        }
    }
}