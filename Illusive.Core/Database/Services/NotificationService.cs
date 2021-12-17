using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Illusive.Data;
using Illusive.Illusive.Core.Database.Interfaces;
using Illusive.Illusive.Database.Interfaces;
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

        public async Task<List<UserNotification>> GetUnreadNotificationsForUserAsync(ApplicationUser user) {
            if ( user == null ) return null;
            
            var val = await this._notifications.FindAsync(x => x.TargetUser.Equals(user.Id) && x.Read == false);
            return val.ToList();
        }

        public async Task<List<UserNotification>> GetUnreadNotificationsForUserIdAsync(Guid id) {
            var val = await this._notifications.FindAsync(x => x.TargetUser.Equals(id) && x.Read == false);
            return val.ToList();
        }
        
        public async Task<List<UserNotification>> GetUnreadNotificationsForUserIdAsync(string id) {
            var val = await this._notifications.FindAsync(x => x.TargetUser.ToString() == id && x.Read == false);
            return val.ToList();
        }

        public async Task UpdateNotificationsWhereAsync(Expression<Func<UserNotification, bool>> expression, UpdateDefinition<UserNotification> update) {
            await this._notifications.UpdateManyAsync(expression, update);
        }
    }
}