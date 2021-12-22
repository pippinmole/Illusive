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
            _logger = logger;
            _forumService = forumService;
            _recaptchaService = recaptchaService;
        }

        public IActionResult OnGet(string id) {
            NewForumData = _forumService.GetForumById(id);

            if ( NewForumData == null || User.GetUniqueId() != NewForumData.OwnerId ) {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id) {
            if ( !User.IsLoggedIn() || !ModelState.IsValid )
                return Page();

            var forumPost = NewForumData;
            if ( !User.CanEditPost(forumPost) && !User.IsAdminAccount() ) {
                _logger.LogError(
                    $"{User.GetUniqueId()} is attempting to edit post {forumPost.Id} but has no access!");
                return Forbid();
            }

            var result = await _recaptchaService.Validate(Request);
            if ( !result.success ) {
                ModelState.AddModelError("", "Recaptcha failed.");
                return Page();
            }

            var user = HttpContext.User;
            if ( !user.IsLoggedIn() ) {
                _logger.LogWarning("Redirecting unauthenticated user to /Index");
                return RedirectToPage("/Index");
            }

            if ( forumPost.Tags != null ) {
                forumPost.Tags = forumPost.Tags.ToLower();

                // Sanitise tags
                var tags = forumPost.Tags.Trim().Split(',');
                if ( tags.Any(string.IsNullOrEmpty) ) {
                    ModelState.AddModelError("", "The forum tag format is invalid.");
                    return Page();
                }

                if ( tags.Distinct().Count() != tags.Length ) {
                    ModelState.AddModelError("", "The forum has duplicate tags.");
                    return Page();
                }
            }

            // Replace it with the user's new (clean) input values.
            var update = Builders<ForumData>.Update.Set(x => x.Content, forumPost.Content)
                .Set(x => x.Tags, forumPost.Tags)
                .Set(x => x.Title, forumPost.Title);
            await _forumService.UpdateForumAsync(id, update);

            _logger.LogInformation(
                $"User {user.GetUniqueId()} updated post titled {forumPost.Title} ({forumPost.Id})");

            return LocalRedirect($"/ForumPost?id={id}");
        }
    }
}