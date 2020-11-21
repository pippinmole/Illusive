using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Database;
using Illusive.Utility;
using MongoDB.Driver;

namespace Illusive.Models.Extensions {
    public static class ForumServiceExtensionMethods {

        public static ForumData GetForumById(this IForumService service, string id) {
            return service.GetForumWhere(x => x.Id == id);
        }

        public static void AddReplyToForum(this IForumService service, ForumData forum, ForumReply forumReply) {
            var newReplies = forum.Replies.ToList();
            newReplies.Add(forumReply);

            var update = Builders<ForumData>.Update.Set(x => x.Replies, newReplies);
            service.UpdateForum(forum.Id, update);
        }

        public static void AddViewToForum(this IForumService service, ForumData forum) {
            var newViews = forum.Views + 1;

            var update = Builders<ForumData>.Update.Set(x => x.Views, newViews);
            service.UpdateForum(forum.Id, update);
        }
        
        public static void AddLikeToForum(this IForumService service, ForumData forum, ClaimsPrincipal user) {
            var userId = user.GetUniqueId();
            forum.Likes.Add(userId);
            
            var update = Builders<ForumData>.Update.Set(x => x.Likes, forum.Likes);
            service.UpdateForum(forum.Id, update);
        }
        
        public static void RemoveLikeFromForum(this IForumService service, ForumData forum, ClaimsPrincipal user) {
            var userId = user.GetUniqueId();
            forum.Likes.RemoveAll(x => x == userId);
            
            var update = Builders<ForumData>.Update.Set(x => x.Likes, forum.Likes);
            service.UpdateForum(forum.Id, update);
        }
        
        public static void RemoveReplyFromForum(this IForumService service, ForumData forum, string replyId) {
            var newReplies = forum.Replies;
            newReplies.RemoveAll(x => x.Id == replyId);
            
            var update = Builders<ForumData>.Update.Set(x => x.Replies, newReplies);
            service.UpdateForum(forum.Id, update);
        }
        
        public static void SetLockState(this IForumService service, ForumData forum, bool value) {
            var update = Builders<ForumData>.Update.Set(x => x.IsLocked, value);
            service.UpdateForum(forum.Id, update);
        }
        public static void ToggleLockState(this IForumService service, ForumData forum) {
            Console.WriteLine($"Setting forum lock state to {!forum.IsLocked}");
            var update = Builders<ForumData>.Update.Set(x => x.IsLocked, !forum.IsLocked);
            service.UpdateForum(forum.Id, update);
        }
    }
}