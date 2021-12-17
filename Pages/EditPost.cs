using System.Linq;
using System.Threading.Tasks;
using Illusive.Database;
using Illusive.Models;
using Illusive.Models.Extensions;
using Illusive.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using reCAPTCHA.AspNetCore;

namespace Illusive.Pages {
    public class EditPostModel : PageModel {
        private readonly ILogger<EditPostModel> _logger;
        private readonly IForumService _forumService;
        private readonly IRecaptchaService _recaptchaService;

        [BindProperty] public ForumData NewForumData { get; set; }

        public EditPostModel(ILogger<EditPostModel> logger, IForumService forumService, IRecaptchaService recaptchaService) {
            this._logger = logger;
            this._forumService = forumService;
            this._recaptchaService = recaptchaService;
        }

        public IActionResult OnGet(string id) {
            this.NewForumData = this._forumService.GetForumById(id);

            if ( this.NewForumData == null || this.User.GetUniqueId() != this.NewForumData.OwnerId ) {
                return this.Forbid();
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(string id) {
            if ( !this.User.IsLoggedIn() || !this.ModelState.IsValid )
                return this.Page();

            var forumPost = this.NewForumData;
            if ( !this.User.CanEditPost(forumPost) && !this.User.IsAdminAccount() ) {
                this._logger.LogError(
                    $"{this.User.GetUniqueId()} is attempting to edit post {forumPost.Id} but has no access!");
                return this.Forbid();
            }

            var result = await this._recaptchaService.Validate(this.Request);
            if ( !result.success ) {
                this.ModelState.AddModelError("", "Recaptcha failed.");
                return this.Page();
            }

            var user = this.HttpContext.User;
            if ( !user.IsLoggedIn() ) {
                this._logger.LogWarning("Redirecting unauthenticated user to /Index");
                return this.RedirectToPage("/Index");
            }

            if ( forumPost.Tags != null ) {
                forumPost.Tags = forumPost.Tags.ToLower();

                // Sanitise tags
                var tags = forumPost.Tags.Trim().Split(',');
                if ( tags.Any(string.IsNullOrEmpty) ) {
                    this.ModelState.AddModelError("", "The forum tag format is invalid.");
                    return this.Page();
                }

                if ( tags.Distinct().Count() != tags.Length ) {
                    this.ModelState.AddModelError("", "The forum has duplicate tags.");
                    return this.Page();
                }
            }

            // Replace it with the user's new (clean) input values.
            var update = Builders<ForumData>.Update.Set(x => x.Content, forumPost.Content)
                .Set(x => x.Tags, forumPost.Tags)
                .Set(x => x.Title, forumPost.Title);
            await this._forumService.UpdateForumAsync(id, update);

            this._logger.LogInformation(
                $"User {user.GetUniqueId()} updated post titled {forumPost.Title} ({forumPost.Id})");

            return this.LocalRedirect($"/ForumPost?id={id}");
        }
    }
}