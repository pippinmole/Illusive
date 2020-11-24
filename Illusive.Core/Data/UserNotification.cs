#nullable enable
using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Illusive.Data {
    public class UserNotification {
        
        [BsonId]
        public string Id { get; set; }
        
        public Guid TargetUser { get; set; }
        public bool Read { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime TimeCreated { get; set; }
        public string? Link { get; set; }
        
        public string TriggerId { get; set; }
        
        public UserNotification(Guid target, string content, string imageUrl, string triggerId, string? link = null) {
            this.Id = Guid.NewGuid().ToString();
            this.TargetUser = target;
            this.Read = false;
            this.Content = content;
            this.TimeCreated = DateTime.Now;
            this.ImageUrl = imageUrl;
            this.Link = link;
            this.TriggerId = triggerId;
        }
    }
}