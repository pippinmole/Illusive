using System.Collections.Generic;
using System.Threading.Tasks;
using Illusive.Illusive.Database.Models;

namespace Illusive.Illusive.Database.Interfaces {
    public interface IForumService {

        Task AddForumPost(ForumData forumPost);
        Task<List<ForumData>> GetForumData();
        ForumData GetForumById(string id);
        void AddReplyToForum(ForumData forum, ForumReply forumReply);
    }
}