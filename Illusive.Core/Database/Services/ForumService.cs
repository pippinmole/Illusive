﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Illusive.Database {
    public class ForumService : IForumService {
        private readonly ILogger<ForumService> _logger;

        private readonly IMongoCollection<ForumData> _forumData;

        public ForumService(ILogger<ForumService> logger, IDatabaseContext ctx) {
            this._logger = logger;
            this._forumData = ctx.GetDatabase("IllusiveDatabase").GetCollection<ForumData>("forum_posts");
        }

        public async Task AddForumPostAsync(ForumData forumPost) {
            await this._forumData.InsertOneAsync(forumPost);
        }

        public async Task<List<ForumData>> GetForumDataAsync() {
            var val = await this._forumData.FindAsync(x => true);

            return val.ToList();
        }

        public ForumData GetForumById(string id) {
            var result = this._forumData.Find(x => x.Id == id);
            return result.FirstOrDefault();
        }
        
        public ForumData GetForumWhere(Expression<Func<ForumData, bool>> expression) {
            return this._forumData.Find(expression).FirstOrDefault();
        }

        public IReadOnlyList<ForumData> GetForumsWhere(Expression<Func<ForumData, bool>> expression) {
            return this._forumData.Find(expression).ToList();
        }

        public void UpdateForum(ForumData forumData, UpdateDefinition<ForumData> update) {
            this._forumData.UpdateOne(x => x.Id == forumData.Id, update);
        }

        public void DeleteForum(Expression<Func<ForumData, bool>> expression) {
            this._forumData.DeleteMany(expression);
        }
    }
}