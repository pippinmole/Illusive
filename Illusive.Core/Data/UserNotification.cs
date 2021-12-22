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
            Id = Guid.NewGuid().ToString();
            TargetUser = target;
            Read = false;
            Content = content;
            TimeCreated = DateTime.Now;
            ImageUrl = imageUrl;
            Link = link;
            TriggerId = triggerId;
        }
    }
}