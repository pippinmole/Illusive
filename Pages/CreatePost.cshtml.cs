using System.Linq;
using System.Threading.Tasks;
using Illusive.Database;
using Illusive.Models;
using Illusive.Models.Extensions;
using Illusive.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using reCAPTCHA.AspNetCore;

namespace Illusive.Pages {
    public class CreatePostModel : PageModel {
        private readonly ILogger<CreatePostModel> _logger;
        private readonly IForumService _forumService;
        private readonly IRecaptchaService _recaptchaService;

        [BindProperty] public ForumData NewForumData { get; set; } = new ForumData();

        public CreatePostModel(ILogger<CreatePostModel> logger, IForumService forumService, IRecaptchaService recaptchaService) {
            _logger = logger;
            _forumService = forumService;
            _recaptchaService = recaptchaService;
        }
        
        public IActionResult OnGet() {
            if ( !User.IsLoggedIn() )
                return LocalRedirect("/");

            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync() {
            if ( !User.IsLoggedIn() || !ModelState.IsValid )
                return Page();

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

            var forumPost = NewForumData.AssignOwner(user);
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

            await _forumService.AddForumPostAsync(forumPost);
            
            _logger.LogInformation($"User {user.GetUniqueId()} created post titled {forumPost.Title}");

            return Redirect("/Forum");
        }
    }
}