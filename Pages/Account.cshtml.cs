#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using Illusive.Database;
using Illusive.Data;
using Illusive.Models;
using Illusive.Utility;
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

        public IActionResult OnGet() {
            var accountId = (string)this.HttpContext.Request.RouteValues["id"];

            if ( string.IsNullOrEmpty(accountId) )
                return this.Redirect($"/Account/{this.User.GetUniqueId()}");

            var account = this._accountService.GetAccountWhere(x => x.Id == accountId);
            if ( account == null ) return this.Redirect("/");

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
                
                this._logger.LogInformation($"User {accountId} changed profile biography to {profileBio}");
            }
            
            var profilePic = accountUpdate.ProfilePicture;
            if ( profilePic != null && profilePic.Length > 0 ) {
                var filePath = Path.GetTempFileName();

                await using var stream = System.IO.File.Create(filePath, (int) profilePic.Length);
                await profilePic.CopyToAsync(stream);

                var path = await this._contentService.UploadFileAsync(Path.GetFileName(profilePic.FileName), stream);

                var update = Builders<AccountData>.Update.Set(x => x.ProfilePicture, path);
                this._accountService.UpdateAccount(accountId, update);
                
                this._logger.LogInformation($"User {accountId} changed profile picture to {path}");
            }

            return this.Page();
        }
    }
}