using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Illusive.Database.Models;
using MongoDB.Driver;

namespace Illusive.Illusive.Database.Interfaces {
    public interface IForumService {

        Task AddForumPostAsync(ForumData forumPost);
        Task<List<ForumData>> GetForumDataAsync();
        
        ForumData GetForumWhere(Expression<Func<ForumData, bool>> expression);
        IReadOnlyList<ForumData> GetForumsWhere(Expression<Func<ForumData, bool>> expression);

        void UpdateForum(ForumData forumData, UpdateDefinition<ForumData> update);

        void DeleteForum(Expression<Func<ForumData, bool>> expression);
    }
}