using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Illusive.Data;
using MongoDB.Driver;

namespace Illusive.Illusive.Core.Database.Interfaces {
    public interface INotificationService {

        Task AddNotificationAsync(UserNotification notification);
        Task<List<UserNotification>> GetNotificationsWhereAsync(Expression<Func<UserNotification, bool>> expression);
        Task<List<UserNotification>> GetUnreadNotificationsForUserAsync(ApplicationUser user);
        Task<List<UserNotification>> GetUnreadNotificationsForUserIdAsync(string id);
        Task<List<UserNotification>> GetUnreadNotificationsForUserIdAsync(Guid id);

        Task UpdateNotificationsWhereAsync(Expression<Func<UserNotification, bool>> expression,
            UpdateDefinition<UserNotification> update);
    }
}