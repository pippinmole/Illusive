using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Illusive.Models;
using MongoDB.Driver;

namespace Illusive.Database {
    public interface IForumService {
        Task AddForumPostAsync(ForumData forumPost);
        Task<List<ForumData>> GetForumDataAsync();
        
        ForumData GetForumWhere(Expression<Func<ForumData, bool>> expression);
        IReadOnlyList<ForumData> GetForumsWhere(Expression<Func<ForumData, bool>> expression);

        void UpdateForum(string id, UpdateDefinition<ForumData> update);
        Task UpdateForumAsync(string id, UpdateDefinition<ForumData> update);

        void DeleteForum(Expression<Func<ForumData, bool>> expression);
    }
}