﻿using System.Linq;
using System.Threading.Tasks;
using Illusive.Illusive.Authentication.Utility;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class CreatePostModel : PageModel {
        private readonly ILogger<CreatePostModel> _logger;
        private readonly IForumService _forumService;

        [BindProperty] public ForumData NewForumData { get; set; } = new ForumData();

        public CreatePostModel(ILogger<CreatePostModel> logger, IForumService forumService) {
            this._logger = logger;
            this._forumService = forumService;
        }
        
        public void OnGet() {
            
        }
        
        public async Task<IActionResult> OnPostAsync() {
            if ( !this.ModelState.IsValid )
                return this.Page();

            var user = this.HttpContext.User;
            if ( !user.IsLoggedIn() ) {
                this._logger.LogWarning("Redirecting unauthenticated user to /Index");
                return this.RedirectToPage("/Index");
            }

            var forumPost = this.NewForumData.AssignOwner(user);
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
            
            this._logger.LogInformation($"User {user.GetUniqueId()} created post titled {forumPost.Title}");

            return this.Redirect("/Forum");
        }
    }
}