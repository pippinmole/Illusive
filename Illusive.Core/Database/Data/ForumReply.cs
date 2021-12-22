using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Illusive.Models {
    public class ForumReply {
        [BsonId]
        public string Id { get; set; }
        
        public Guid AuthorId { get; set; }
        [Required] [StringLength(1000, MinimumLength = 10)] public string Content { get; set; }
        
        public DateTime TimeCreated { get; set; }

        public ForumReply() {
            Id = Guid.NewGuid().ToString();
            TimeCreated = DateTime.Now;
        }
    }
}