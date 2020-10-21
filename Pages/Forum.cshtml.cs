using System;
using System.Collections.Generic;
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
            var user = this.HttpContext.User;
            if ( !user.Identity.IsAuthenticated ) {
                this._logger.LogWarning("Redirecting unauthenticated user to /Index");
                return this.RedirectToPage("/Index");
            }

            var forumPost = this.ForumData.AssignOwner(user);
            
            await this._forumService.AddForumPost(forumPost);
            
            this._logger.LogWarning($"Created a forum post titled: {this.ForumData.Title}");

            return this.Redirect("/Forum");
        }

        public async Task<List<ForumData>> GetForums() {
            return await this._forumService.GetForumData();
        }
    }
}