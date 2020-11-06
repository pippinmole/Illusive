#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Illusive.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Illusive.Illusive.Database.Models {
    public class ForumData {

        [BsonId] public string Id { get; set; }

        [Required, StringLength(80, MinimumLength = 15)] public string Title { get; set; }

        [Required, StringLength(10000, MinimumLength = 25)] public string Content { get; set; }

        [StringLength(100)] public string? Tags { get; set; } = "";
        
        public string? OwnerId { get; set; } // Will be null in the user-request, but not when stored in the database.
        public DateTime TimeCreated { get; set;  }
        public List<ForumReply> Replies { get; set; }
        public uint Views { get; set; }
        public List<string> Likes { get; set; }

        public ForumData() {
            this.Id = Guid.NewGuid().ToString();
            this.Replies = new List<ForumReply>();
            this.TimeCreated = DateTime.Now;
            this.Views = 0;
            this.Likes = new List<string>();
        }
    }
}