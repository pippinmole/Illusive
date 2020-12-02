using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Illusive.Data;
using Illusive.Illusive.Core.User_Management.Extension_Methods;
using Illusive.Illusive.Core.User_Management.Interfaces;
using Illusive.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class AccountChangeModel : PageModel {
        
        [BindProperty] public AccountChangeData AccountUpdate { get; set; }

        private readonly ILogger<AccountChangeModel> _logger;
        private readonly IAppUserManager _userManager;
        private readonly IContentService _contentService;
        
        public AccountChangeModel(ILogger<AccountChangeModel> logger, IAppUserManager userManager, IContentService contentService) {
            this._logger = logger;
            this._userManager = userManager;
            this._contentService = contentService;
        }
        
        public async Task<IActionResult> OnPostUploadAsync() {
            if ( !this.ModelState.IsValid )
                return this.Page();

            var accountId = this.User.GetUniqueId();
            var user = await this._userManager.GetUserByIdAsync(accountId);
            var newAccount = this.AccountUpdate ?? throw new NullReferenceException("Account Change data shouldn't be null!");

            user.Bio = newAccount.Bio;
            user.Location = newAccount.Location;

            user.GithubUrl = newAccount.GithubUrl;
            user.LinkedinUrl = newAccount.LinkedInUrl;
            user.RedditUrl = newAccount.RedditUrl;
            user.TwitterUrl = newAccount.TwitterUrl;
            user.SteamUrl = newAccount.SteamUrl;
            
            var profilePic = newAccount.ProfilePicture;
            if ( profilePic != null && profilePic.Length > 0 ) {
                await using var stream = System.IO.File.Create(Path.GetTempFileName(), (int) profilePic.Length);
                await profilePic.CopyToAsync(stream);

                var path = await this._contentService.UploadFileAsync(Path.GetFileName(profilePic.FileName), stream);
                user.ProfilePicture = path;
            }

            if ( newAccount.CoverPicture != null )
                user.CoverPicture = await this._contentService.UploadFileAsync(newAccount.CoverPicture);

            if ( newAccount.ProfilePicture != null )
                user.ProfilePicture = await this._contentService.UploadFileAsync(newAccount.ProfilePicture);

            await this._userManager.UpdateUserAsync(user);

            return this.RedirectToPage("Account");
        }
    }
}