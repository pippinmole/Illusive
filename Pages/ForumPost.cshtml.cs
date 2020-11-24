using System;
using System.Linq;
using System.Threading.Tasks;
using Illusive.Data;
using Illusive.Database;
using Illusive.Illusive.Core.Database.Interfaces;
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
        private readonly INotificationService _notificationService;

        public ForumData ForumData { get; set; }
        [BindProperty] public ForumReply ForumReply { get; set; }

        public ForumPost(ILogger<ForumPost> logger, IForumService forumService,
            INotificationService notificationService) {
            this._logger = logger;
            this._forumService = forumService;
            this._notificationService = notificationService;
        }

        public async Task<IActionResult> OnGetAsync(string id) {
            this.ForumData = this._forumService.GetForumById(id);

            if ( this.ForumData == null )
                return this.LocalRedirect("/Index");

            this._forumService.AddViewToForum(this.ForumData);
            await this._notificationService.ReadNotificationsForUserAsync(this.User, this.ForumData.Id);

            return this.Page();
        }

        public async Task<IActionResult> OnPost(string id) {
            this.ForumData = this._forumService.GetForumById(id);

            if ( !this.ModelState.IsValid )
                return this.Page();

            var forum = this._forumService.GetForumById(id);
            var reply = this.ForumReply;
            // TODO: Fix this
            // reply.AuthorId = this.User.GetUniqueId();

            if ( forum.IsLocked ) {
                this._logger.LogError($"{this.User.GetDisplayName()} tried to reply to a locked forum! This shouldn't be possible.");
                return this.Forbid();
            }
            
            this._forumService.AddReplyToForum(forum, reply);

            // Don't notify the owner of their own comments!
            if ( reply.AuthorId != forum.OwnerId ) {
                await this._notificationService.AddNotificationAsync(new UserNotification(
                    target: forum.OwnerId,
                    content:
                    $"{this.User.GetDisplayName()} has commented to your post: {reply.Content.SafeSubstring(0, 25)}...",
                    imageUrl: "",
                    link: this.Request.Path + this.Request.QueryString,
                    triggerId: forum.Id
                ));
            }

            this._logger.LogInformation($"{this.User.GetUniqueId()} has replied to forum post {forum.Id}");

            return this.Redirect($"/ForumPost?id={id}");
        }

        public class ForumLike {
            public string forumId { get; set; }
        }

        public class ForumLock {
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

            if ( forum == null ) {
                this._logger.LogWarning(
                    $"{this.User.GetUniqueId()} is trying to delete an invalid forum ({body.forumId})");
                return this.BadRequest();
            }

            if ( user.CanDeletePost(forum) || user.IsAdminAccount() ) {
                this._forumService.DeleteForum(x => x.Id == forum.Id);
                this._logger.LogInformation(
                    $"{this.User.GetDisplayName()} has successfully deleted forum with id {forum.Id}!");
            } else {
                this._logger.LogWarning(
                    $"{this.User.GetDisplayName()} is attempting to delete a forum without being authenticated!");
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
            if ( !this.User.IsLoggedIn() )
                this._logger.LogWarning("User attempting to delete a forum without being authenticated!");

            var forum = this._forumService.GetForumById(body.forumId);
            var reply = forum.Replies.FirstOrDefault(x => x.Id == body.replyId);
            var user = this.User;

            if ( reply == null ) {
                return new JsonResult(new {Error = "TRUE"});
            }

            if ( user.CanDeleteReply(reply) || user.IsAdminAccount() ) {
                this._forumService.RemoveReplyFromForum(forum, reply.Id);
                this._logger.LogInformation(
                    $"{this.User.GetDisplayName()} has successfully deleted forum reply with id {reply.Id}!");
            } else {
                this._logger.LogWarning(
                    $"{this.User.GetDisplayName()} is attempting to delete a forum reply without being authenticated!");
            }

            return new JsonResult(new {
                Redirect = $"/ForumPost?id={forum.Id}"
            });
        }

        // POST: LockPost?handler={json}
        public ActionResult OnPostLockPost([FromBody] ForumLock body) {
            if ( !this.User.IsLoggedIn() )
                this._logger.LogWarning("User attempting to delete a forum without being authenticated!");

            var forum = this._forumService.GetForumById(body.forumId);
            var user = this.User;

            this._logger.LogWarning("OnLockPost");
            
            if ( user.CanLockPost(forum) || user.IsAdminAccount() ) {
                this._forumService.ToggleLockState(forum);

                this._logger.LogInformation(
                    forum.IsLocked
                        ? $"{this.User.GetDisplayName()} has successfully unlocked forum reply with id {forum.Id}!"
                        : $"{this.User.GetDisplayName()} has successfully locked forum reply with id {forum.Id}!");
            } else {
                this._logger.LogWarning(
                    $"{this.User.GetDisplayName()} is attempting to lock a forum without being authenticated!");
            }

            return new JsonResult(new {
                Redirect = $"/ForumPost?id={forum.Id}"
            });
        }
    }
}