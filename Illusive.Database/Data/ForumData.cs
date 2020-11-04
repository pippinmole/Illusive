#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Illusive.Illusive.Database.Models {
    public class ForumData {

        [BsonId] public string Id { get; set; }

        [Required, StringLength(80)] public string Title { get; set; }

        [Required, MaxLength(10000)] public string Content { get; set; }

        [MaxLength(100)] public string? Tags { get; set; } = "";
        
        public string? OwnerId { get; set; } // Will be null in the user-request, but not when stored in the database.
        public DateTime TimeCreated { get; set;  }
        public List<ForumReply> Replies { get; set; }
        public uint Views { get; set; }

        public ForumData() {
            this.Id = Guid.NewGuid().ToString();
            this.Replies = new List<ForumReply>();
            this.TimeCreated = DateTime.Now;
            this.Views = 0;
        }
    }
}