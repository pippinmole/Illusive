﻿using System;
using System.Linq;
using Illusive.Database;
using Illusive.Models;
using Illusive.Models.Extensions;
using Illusive.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Illusive.Pages {
    public class ForumPost : PageModel {

        private readonly ILogger<ForumPost> _logger;
        private readonly IForumService _forumService;
        private readonly IAccountService _accountService;

        public ForumData ForumData { get; set; }
        [BindProperty] public ForumReply ForumReply { get; set; }

        public ForumPost(ILogger<ForumPost> logger, IForumService forumService, IAccountService accountService) {
            this._logger = logger;
            this._forumService = forumService;
            this._accountService = accountService;
        }

        public IActionResult OnGet(string id) {
            this.ForumData = this._forumService.GetForumById(id);
            
            if ( this.ForumData == null )
                return this.LocalRedirect("/Index");

            this._forumService.AddViewToForum(this.ForumData);

            return this.Page();
        }

        public IActionResult OnPost(string id) {
            if ( !this.ModelState.IsValid ) {
                return this.Redirect($"/ForumPost?id={id}");
            }

            var forum = this._forumService.GetForumById(id);
            var reply = this.ForumReply;
            reply.AuthorId = this.User.GetUniqueId();

            this._forumService.AddReplyToForum(forum, reply);

            this._logger.LogInformation($"{this.User.GetUniqueId()} has replied to forum post {forum.Id}");
            
            return this.Redirect($"/ForumPost?id={id}");
        }

        public class ForumLike {
            public string forumId { get; set; }
        }
        
        public class ForumDelete {
            public string forumId { get; set; }
        }
        
        public class ForumReplyDelete {
            public string forumId { get; set; }
            public string replyId { get; set; }
        }

        // POST: DeletePost?handler={json}
        public ActionResult OnPostDeletePost([FromBody] ForumDelete body) {
            if ( !this.User.IsLoggedIn() )
                this._logger.LogWarning("User attempting to delete a forum without being authenticated!");
            
            var forum = this._forumService.GetForumById(body.forumId);
            var user = this.User;

            if ( user.CanDeletePost(forum) || user.IsAdminAccount() ) {
                this._forumService.DeleteForum(x => x.Id == forum.Id);
                this._logger.LogInformation($"{this.User.GetDisplayName()} has successfully deleted forum with id {forum.Id}!");
            } else {
                this._logger.LogWarning($"{this.User.GetDisplayName()} is attempting to delete a forum without being authenticated!");
            }

            return new JsonResult(new {
                Redirect = "/"
            });
        }
        
        // POST: LikePost?handler={json}
        public ActionResult OnPostLikePost([FromBody] ForumLike body) {
            if ( !this.User.IsLoggedIn() )
                throw new Exception("User attempting to like a forum without being authenticated!");
            
            var forum = this._forumService.GetForumById(body.forumId);
            var user = this.User;

            var isLiked = forum.Likes.Contains(user.GetUniqueId());
            if ( isLiked ) {
                this._logger.LogInformation($"{user.GetUniqueId()} is unliking post {body.ToJson()})");
                this._forumService.RemoveLikeFromForum(forum, user);
            } else {
                this._logger.LogInformation($"{user.GetUniqueId()} is liking post {body.ToJson()})");
                this._forumService.AddLikeToForum(forum, user);
            }

            return new JsonResult(new {
                Result = isLiked ? "unliked" : "liked",
                LikeCount = forum.Likes.Count
            });
        }
        
        // POST: DeleteReply?handler={json}
        public ActionResult OnPostDeleteReply([FromBody] ForumReplyDelete body) {
            
            this._logger.LogWarning($"Attempting to delete reply id {body.replyId}");
            
            if ( !this.User.IsLoggedIn() )
                this._logger.LogWarning("User attempting to delete a forum without being authenticated!");
            
            var forum = this._forumService.GetForumById(body.forumId);
            var reply = forum.Replies.FirstOrDefault(x => x.Id == body.replyId);
            var user = this.User;

            if ( reply == null ) {
                return new JsonResult(new { Error = "TRUE" });
            }
            
            if ( user.CanDeleteReply(reply) || user.IsAdminAccount() ) {
                this._forumService.RemoveReplyFromForum(forum, reply.Id);
                this._logger.LogInformation($"{this.User.GetDisplayName()} has successfully deleted forum reply with id {reply.Id}!");
            } else {
                this._logger.LogWarning($"{this.User.GetDisplayName()} is attempting to delete a forum reply without being authenticated!");
            }

            return new JsonResult(new {
                Redirect = $"/ForumPost?id={forum.Id}"
            });
        }
    }
}