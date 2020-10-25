using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        // public async void OnGetAsync() {
        //     var user = this.HttpContext.User;
        //     if ( !user.Identity.IsAuthenticated ) {
        //         await this.HttpContext.ChallengeAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //         this._logger.LogWarning("Challenge done");
        //     }
        // }

        public async Task<IActionResult> OnPostAsync() {
            
            if ( !this.ModelState.IsValid )
                return this.Page();

            var user = this.HttpContext.User;
            if ( !user.Identity.IsAuthenticated ) {
                this._logger.LogWarning("Redirecting unauthenticated user to /Index");
                return this.RedirectToPage("/Index");
            }

            var forumPost = this.ForumData.AssignOwner(user);

            if ( forumPost.Title.Length > 100 ) {
                this.ModelState.AddModelError("", "The forum title has to be under 100 characters.");
                return this.Page();
            } 
            if ( forumPost.Content.Length > 10000 ) {
                this.ModelState.AddModelError("", "The forum contents has to be under 10,000 characters.");
                return this.Page();
            }
            
            await this._forumService.AddForumPost(forumPost);
            
            this._logger.LogWarning($"Created a forum post titled: {this.ForumData.Title}");

            return this.Redirect("/Forum");
        }

        public async Task<List<ForumData>> GetForums() {
            return await this._forumService.GetForumData();
        }
    }
}