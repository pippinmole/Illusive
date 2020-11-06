using System;
using System.ComponentModel.DataAnnotations;
using Illusive.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Illusive.Illusive.Database.Models {
    public class ForumReply {
        [BsonId]
        public string Id { get; set; }
        
        public string AuthorId { get; set; }
        [Required] [StringLength(1000, MinimumLength = 10)] public string Content { get; set; }
        
        public DateTime TimeCreated { get; set; }

        public ForumReply() {
            this.TimeCreated = DateTime.Now;
        }
    }
}