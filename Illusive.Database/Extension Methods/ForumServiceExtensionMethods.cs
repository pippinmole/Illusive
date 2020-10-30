using System.Linq;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Illusive.Database.Models;
using MongoDB.Driver;

namespace Illusive.Illusive.Database.Extension_Methods {
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
    }
}