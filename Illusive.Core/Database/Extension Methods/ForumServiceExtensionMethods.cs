using System.Linq;
using System.Security.Claims;
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
            service.UpdateForum(forum, update);
        }

        public static void AddViewToForum(this IForumService service, ForumData forum) {
            var newViews = forum.Views + 1;

            var update = Builders<ForumData>.Update.Set(x => x.Views, newViews);
            service.UpdateForum(forum, update);
        }
        
        public static void AddLikeToForum(this IForumService service, ForumData forum, ClaimsPrincipal user) {
            var userId = user.GetUniqueId();
            forum.Likes.Add(userId);
            
            var update = Builders<ForumData>.Update.Set(x => x.Likes, forum.Likes);
            service.UpdateForum(forum, update);
        }
        
        public static void RemoveLikeFromForum(this IForumService service, ForumData forum, ClaimsPrincipal user) {
            var userId = user.GetUniqueId();
            forum.Likes.RemoveAll(x => x == userId);
            
            var update = Builders<ForumData>.Update.Set(x => x.Likes, forum.Likes);
            service.UpdateForum(forum, update);
        }
    }
}