using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public ForumModel(ILogger<ForumModel> logger, IConfiguration configuration, IForumService forumService) {
            this._logger = logger;
            this._forumService = forumService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync() {
            this._logger.LogWarning("Creating a forum post");

            var post = new ForumData(Guid.NewGuid().ToString().Substring(0, 10), "Test Forum post!",
                "A test content body for a forum post!");
            await this._forumService.AddForumPost(post);
            
            this._logger.LogWarning($"Created a forum post titled: {post.Title}");

            return this.Page();
        }

        public async Task<List<ForumData>> GetForums() {
            return await this._forumService.GetForumData();
        }
    }
}