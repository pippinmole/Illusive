using System;
using System.Collections.Generic;
using Illusive.Illusive.Authentication.Utility;
using Illusive.Illusive.Database.Extension_Methods;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Illusive.Pages {
    public class ForumPost : PageModel {

        private readonly ILogger<ForumPost> _logger;
        private readonly IForumService _forumService;
        private readonly IAccountService _accountService;

        [BindProperty] public ForumData ForumPostData { get; set; }
        [BindProperty] public ForumReply ForumReply { get; set; }

        public ForumPost(ILogger<ForumPost> logger, IForumService forumService, IAccountService accountService) {
            this._logger = logger;
            this._forumService = forumService;
            this._accountService = accountService;
        }

        public IActionResult OnGet(string id) {
            this.ForumPostData = this._forumService.GetForumById(id);

            if ( this.ForumPostData == null ) {
                return this.LocalRedirect("/Index");
            }
            this._forumService.AddViewToForum(this.ForumPostData);

            return this.Page();
        }

        public IActionResult OnPost(string id) {
            var forum = this._forumService.GetForumById(id);
            var reply = this.ForumReply;
            reply.AuthorId = this.User.GetUniqueId();

            this._forumService.AddReplyToForum(forum, reply);

            return this.Redirect($"/ForumPost?id={id}");
        }

        public class ForumLike {
            public string forumId { get; set; }
        }

        // POST: LikePost?handler={json}
        public ActionResult OnPostLikePost([FromBody] ForumLike body) {
            if ( !this.User.Identity.IsAuthenticated )
                throw new Exception("User attempting to like a forum without being authenticated!");
            
            var forum = this._forumService.GetForumById(body.forumId);
            var user = this.User;

            var isLiked = forum.Likes.Contains(user.GetUniqueId());
            if ( isLiked ) {
                this._logger.LogWarning($"{user.GetUniqueId()} is unliking post {body.ToJson()})");
                this._forumService.RemoveLikeFromForum(forum, user);
            } else {
                this._logger.LogWarning($"{user.GetUniqueId()} is liking post {body.ToJson()})");
                this._forumService.AddLikeToForum(forum, user);
            }

            return new JsonResult(new {
                Result = isLiked ? "unliked" : "liked",
                LikeCount = forum.Likes.Count
            });
        }
    }
}