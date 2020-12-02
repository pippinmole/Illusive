#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDbCore.Extensions;
using Illusive.Data;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Illusive.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class AccountModel : PageModel {

        private readonly ILogger<AccountModel> _logger;
        private readonly IAppUserManager _userManager;
        private readonly IContentService _contentService;
        
        [BindProperty] public AccountChangeData AccountUpdate { get; set; }
        
        public AccountModel(ILogger<AccountModel> logger, IAppUserManager userManager, IContentService contentService) {
            this._logger = logger;
            this._userManager = userManager ?? throw new NullReferenceException("AccountService needs to be setup!");
            this._contentService = contentService;
        }

        public async Task<IActionResult> OnGetAsync(string? id) {
            if ( id == null || !Guid.TryParse(id, out var guid) ) {
                this._logger.LogInformation($"User attempted to access invalid user id - redirecting...");
                
                if ( this.User.IsLoggedIn() )
                    return this.Redirect($"/Account/{this.User.GetUniqueId()}");

                return this.LocalRedirect("/");
            }

            var account = await this._userManager.GetUserByIdAsync(guid);
            if ( account == null ) return this.Redirect("/");

            return this.Page();
        }

        public class DeleteAccountPost {
            public Guid accountId;
        }
        
        public async Task<IActionResult> OnPostDeleteAccountAsync([FromBody] DeleteAccountPost post) {
            if ( !this.ModelState.IsValid )
                return this.Page();

            var reqAccountId = post.accountId;
            if ( this.User.GetUniqueId() != reqAccountId ) {
                this._logger.LogWarning($"{this.User.GetUniqueId()} tried to delete account of id {reqAccountId}!");
                return this.Forbid();
            }

            var user = await this._userManager.GetUserByIdAsync(reqAccountId);
            var result = await this._userManager.RemoveUserAsync(user);
            
            if ( result.Succeeded ) {
                this._logger.LogInformation($"{user.UserName} has been deleted");

                await this._userManager.SignOutAsync();
            } else {
                this._logger.LogError($"Error when trying to delete user {user.UserName}.");
            }

            return new JsonResult(new {
                Redirect = "/Index"
            });
        }
    }
}