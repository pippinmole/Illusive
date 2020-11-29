using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Illusive.Data;
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
            var accountUpdate = this.AccountUpdate ?? throw new NullReferenceException("Account Change data shouldn't be null!");

            user.GithubUrl = accountUpdate.GithubUrl;
            user.LinkedinUrl = accountUpdate.LinkedInUrl;
            user.RedditUrl = accountUpdate.RedditUrl;
            user.TwitterUrl = accountUpdate.TwitterUrl;
            user.SteamUrl = accountUpdate.SteamUrl;

            await this._userManager.UpdateUserAsync(user);
            
            var profileBio = accountUpdate.Bio;
            if ( !string.IsNullOrEmpty(profileBio) ) {
                user.Bio = accountUpdate.Bio;

                var result = await this._userManager.UpdateUserAsync(user);
                if ( result.Succeeded ) {
                    this._logger.LogInformation($"User {accountId} changed profile biography to {profileBio}");
                } else {
                    this._logger.LogError($"Uncaught error when trying to update {user}'s bio to {profileBio}: {result.Errors.FirstOrDefault()}");
                }
            }
            
            var profilePic = accountUpdate.ProfilePicture;
            if ( profilePic != null && profilePic.Length > 0 ) {
                await using var stream = System.IO.File.Create(Path.GetTempFileName(), (int) profilePic.Length);
                await profilePic.CopyToAsync(stream);

                var path = await this._contentService.UploadFileAsync(Path.GetFileName(profilePic.FileName), stream);
                user.ProfilePicture = path;
                
                var result = await this._userManager.UpdateUserAsync(user);
                if ( result.Succeeded ) {
                    this._logger.LogInformation($"User {accountId} changed profile picture to {path}");
                } else {
                    this._logger.LogError($"Uncaught error when trying to update {user}'s profile picture to {path}: {result.Errors.FirstOrDefault()}");
                }
            }
            
            var coverPicture = accountUpdate.CoverPicture;
            if ( coverPicture != null && coverPicture.Length > 0 ) {
                await using var stream = System.IO.File.Create(Path.GetTempFileName(), (int) coverPicture.Length);
                await coverPicture.CopyToAsync(stream);

                var path = await this._contentService.UploadFileAsync(Path.GetFileName(coverPicture.FileName), stream);
                user.CoverPicture = path;
                
                var result = await this._userManager.UpdateUserAsync(user);
                if ( result.Succeeded ) {
                    this._logger.LogInformation($"User {accountId} changed profile picture to {path}");
                } else {
                    this._logger.LogError($"Uncaught error when trying to update {user}'s profile picture to {path}: {result.Errors.FirstOrDefault()}");
                }
            }

            if ( accountUpdate.Location != user.Location ) {
                user.Location = accountUpdate.Location;
                
                var result = await this._userManager.UpdateUserAsync(user);
                if ( result.Succeeded ) {
                    this._logger.LogInformation($"User {accountId} changed their account location.");
                } else {
                    this._logger.LogError($"Uncaught error when trying to update {user}'s account location.");
                }
            }

            return this.RedirectToPage("Account");
        }
    }
}