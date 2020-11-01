using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class ForumModel : PageModel {

        private readonly ILogger<ForumModel> _logger;
        private readonly IForumService _forumService;

        [BindProperty] public ForumData ForumData { get; set; } = new ForumData();
        
        public ForumModel(ILogger<ForumModel> logger, IConfiguration configuration, IForumService forumService) {
            this._logger = logger;
            this._forumService = forumService;
        }

        public async Task<IActionResult> OnPostAsync() {
            if ( !this.ModelState.IsValid )
                return this.Page();

            var user = this.HttpContext.User;
            if ( !user.Identity.IsAuthenticated ) {
                this._logger.LogWarning("Redirecting unauthenticated user to /Index");
                return this.RedirectToPage("/Index");
            }

            var forumPost = this.ForumData.AssignOwner(user);
            if ( forumPost.Tags != null ) {
                forumPost.Tags = forumPost.Tags.ToLower();

                if ( forumPost.Title.Length > 100 ) {
                    this.ModelState.AddModelError("", "The forum title has to be under 100 characters.");
                    return this.Page();
                }

                if ( forumPost.Content.Length > 10000 ) {
                    this.ModelState.AddModelError("", "The forum contents has to be under 10,000 characters.");
                    return this.Page();
                }

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

            await this._forumService.AddForumPostAsync(forumPost);

            return this.Redirect("/Forum");
        }

        public async Task<List<ForumData>> GetForums() {
            return await this._forumService.GetForumDataAsync();
        }
    }
}