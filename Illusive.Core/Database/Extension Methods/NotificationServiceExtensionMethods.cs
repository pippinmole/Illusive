﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Illusive.Data;
using Illusive.Illusive.Core.Database.Interfaces;
using Illusive.Utility;
using MongoDB.Driver;

namespace Illusive.Models.Extensions {
    public static class NotificationServiceExtensionMethods {
        public static async Task<List<UserNotification>> GetNotificationsForUserAsync(this INotificationService notificationService, ClaimsPrincipal user) {
            return await notificationService.GetNotificationsWhereAsync(x => x.TargetUser == user.GetUniqueId());
        }
        public static async Task ReadNotificationsForUserAsync(this INotificationService notificationService, string triggerId) {
            var update = Builders<UserNotification>.Update.Set(x => x.Read, true);
            await notificationService.UpdateNotificationsWhereAsync(x => x.TriggerId == triggerId, update);
        }
    }
}