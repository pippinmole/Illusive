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
            _logger = logger;
            _forumService = forumService;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> OnGetAsync(string id) {
            ForumData = _forumService.GetForumById(id);

            if ( ForumData == null )
                return LocalRedirect("/Index");

            _forumService.AddViewToForum(ForumData);
            await _notificationService.ReadNotificationsForUserAsync(User, ForumData.Id);

            return Page();
        }

        public async Task<IActionResult> OnPost(string id) {
            ForumData = _forumService.GetForumById(id);

            if ( !ModelState.IsValid )
                return Page();

            var forum = _forumService.GetForumById(id);
            var reply = ForumReply;
            reply.AuthorId = User.GetUniqueId();

            if ( forum.IsLocked ) {
                _logger.LogError($"{User.GetDisplayName()} tried to reply to a locked forum! This shouldn't be possible.");
                return Forbid();
            }
            
            _forumService.AddReplyToForum(forum, reply);

            // Don't notify the owner of their own comments!
            if ( reply.AuthorId != forum.OwnerId ) {
                await _notificationService.AddNotificationAsync(new UserNotification(
                    target: forum.OwnerId,
                    content:
                    $"{User.GetDisplayName()} has commented to your post: {reply.Content.SafeSubstring(0, 25)}...",
                    imageUrl: "",
                    link: Request.Path + Request.QueryString,
                    triggerId: forum.Id
                ));
            }

            _logger.LogInformation($"{User.GetUniqueId()} has replied to forum post {forum.Id}");

            return Redirect($"/ForumPost?id={id}");
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
            if ( !User.IsLoggedIn() )
                _logger.LogWarning("User attempting to delete a forum without being authenticated!");

            var forum = _forumService.GetForumById(body.forumId);
            var user = User;

            if ( forum == null ) {
                _logger.LogWarning(
                    $"{User.GetUniqueId()} is trying to delete an invalid forum ({body.forumId})");
                return BadRequest();
            }

            if ( user.CanDeletePost(forum) || user.IsAdminAccount() ) {
                _forumService.DeleteForum(x => x.Id == forum.Id);
                _logger.LogInformation(
                    $"{User.GetDisplayName()} has successfully deleted forum with id {forum.Id}!");
            } else {
                _logger.LogWarning(
                    $"{User.GetDisplayName()} is attempting to delete a forum without being authenticated!");
            }

            return new JsonResult(new {
                Redirect = "/"
            });
        }

        // POST: LikePost?handler={json}
        public ActionResult OnPostLikePost([FromBody] ForumLike body) {
            if ( !User.IsLoggedIn() )
                throw new Exception("User attempting to like a forum without being authenticated!");

            var forum = _forumService.GetForumById(body.forumId);
            var user = User;

            var isLiked = forum.Likes.Contains(user.GetUniqueId());
            if ( isLiked ) {
                _logger.LogInformation($"{user.GetUniqueId()} is unliking post {body.ToJson()})");
                _forumService.RemoveLikeFromForum(forum, user);
            } else {
                _logger.LogInformation($"{user.GetUniqueId()} is liking post {body.ToJson()})");
                _forumService.AddLikeToForum(forum, user);
            }

            return new JsonResult(new {
                Result = isLiked ? "unliked" : "liked",
                LikeCount = forum.Likes.Count
            });
        }

        // POST: DeleteReply?handler={json}
        public ActionResult OnPostDeleteReply([FromBody] ForumReplyDelete body) {
            if ( !User.IsLoggedIn() )
                _logger.LogWarning("User attempting to delete a forum without being authenticated!");

            var forum = _forumService.GetForumById(body.forumId);
            var reply = forum.Replies.FirstOrDefault(x => x.Id == body.replyId);
            var user = User;

            if ( reply == null ) {
                return new JsonResult(new {Error = "TRUE"});
            }

            if ( user.CanDeleteReply(reply) || user.IsAdminAccount() ) {
                _forumService.RemoveReplyFromForum(forum, reply.Id);
                _logger.LogInformation(
                    $"{User.GetDisplayName()} has successfully deleted forum reply with id {reply.Id}!");
            } else {
                _logger.LogWarning(
                    $"{User.GetDisplayName()} is attempting to delete a forum reply without being authenticated!");
            }

            return new JsonResult(new {
                Redirect = $"/ForumPost?id={forum.Id}"
            });
        }

        // POST: LockPost?handler={json}
        public ActionResult OnPostLockPost([FromBody] ForumLock body) {
            if ( !User.IsLoggedIn() )
                _logger.LogWarning("User attempting to delete a forum without being authenticated!");

            var forum = _forumService.GetForumById(body.forumId);
            var user = User;

            _logger.LogWarning("OnLockPost");
            
            if ( user.CanLockPost(forum) || user.IsAdminAccount() ) {
                _forumService.ToggleLockState(forum);

                _logger.LogInformation(
                    forum.IsLocked
                        ? $"{User.GetDisplayName()} has successfully unlocked forum reply with id {forum.Id}!"
                        : $"{User.GetDisplayName()} has successfully locked forum reply with id {forum.Id}!");
            } else {
                _logger.LogWarning(
                    $"{User.GetDisplayName()} is attempting to lock a forum without being authenticated!");
            }

            return new JsonResult(new {
                Redirect = $"/ForumPost?id={forum.Id}"
            });
        }
    }
}