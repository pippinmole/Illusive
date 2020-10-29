using Illusive.Illusive.Database.Extension_Methods;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Illusive.Pages {
    public class ForumPost : PageModel {
        
        private readonly ILogger<ForumPost> _logger;
        private readonly IForumService _forumService;

        [BindProperty] public ForumData ForumPostData { get; set; }
        [BindProperty] public ForumReply ForumReply { get; set; }

        public ForumPost(ILogger<ForumPost> logger, IForumService forumService) {
            this._logger = logger;
            this._forumService = forumService;
        }
        
        public void OnGet(string id) {
            this.ForumPostData = this._forumService.GetForumById(id);
            this._forumService.AddViewToForum(this.ForumPostData);
            
            this._logger.LogWarning($"Found forum at id {id}: {this.ForumPostData.Title}");
        }

        public IActionResult OnPost(string id) {
            this._logger.LogWarning($"Got post request from {id}");

            var forum = this._forumService.GetForumById(id);
            var reply = this.ForumReply;

            this._forumService.AddReplyToForum(forum, reply);

            return this.Redirect($"/ForumPost?id={id}");
        }
    }
}