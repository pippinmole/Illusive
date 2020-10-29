using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Illusive.Illusive.Database.Models;

namespace Illusive.Illusive.Database.Interfaces {
    public interface IForumService {

        Task AddForumPostAsync(ForumData forumPost);
        Task<List<ForumData>> GetForumDataAsync();
        
        ForumData GetForumWhere(Expression<Func<ForumData, bool>> expression);
        void AddReplyToForum(ForumData forum, ForumReply forumReply);
        void AddViewToForum(ForumData forum);
    }
}