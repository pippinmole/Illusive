using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Data;
using Illusive.Illusive.Core.Database.Interfaces;
using Illusive.Illusive.Database.Interfaces;
using Illusive.Models;
using Illusive.Utility;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Illusive.Database {
    public class NotificationService : INotificationService {

        private readonly ILogger<NotificationService> _logger;
        private readonly IMongoCollection<UserNotification> _notifications;

        public NotificationService(ILogger<NotificationService> logger, IDatabaseContext ctx) {
            this._logger = logger;
            this._notifications = ctx.GetDatabase("IllusiveDatabase").GetCollection<UserNotification>("user_notifications");
        }
        
        public async Task AddNotificationAsync(UserNotification notification) {
            await this._notifications.InsertOneAsync(notification);
        }
        
        public async Task<List<UserNotification>> GetNotificationsWhereAsync(Expression<Func<UserNotification, bool>> expression) {
            var val = await this._notifications.FindAsync(expression);
            return val.ToList();
        }

        public async Task<List<UserNotification>> GetUnreadNotificationsForUserAsync(ClaimsPrincipal user) {
            var val = await this._notifications.FindAsync(x => x.TargetUser == user.GetUniqueId() && x.Read == false);
            return val.ToList();
        }
        
        public async Task<List<UserNotification>> GetUnreadNotificationsForUserAsync(AccountData user) {
            var val = await this._notifications.FindAsync(x => x.TargetUser == user.Id && x.Read == false);
            return val.ToList();
        }

        public async Task UpdateNotificationsWhereAsync(Expression<Func<UserNotification, bool>> expression, UpdateDefinition<UserNotification> update) {
            await this._notifications.UpdateManyAsync(expression, update);
        }
    }
}