#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Illusive.Attributes;
using Illusive.Settings;
using MongoDB.Bson.Serialization.Attributes;

namespace Illusive.Models {
    public class ForumData {

        [BsonId] public string Id { get; set; }

        [Required, UnicodeOnly]
        [StringLength(ForumSettings.MaxTitleLength, MinimumLength = ForumSettings.MinTitleLength)]
        public string Title { get; set; }

        [Required, UnicodeOnly]
        [StringLength(ForumSettings.MaxContentLength, MinimumLength = ForumSettings.MinContentLength)]
        public string Content { get; set; }

        [StringLength(ForumSettings.MaxTagLength), UnicodeOnly]
        public string? Tags { get; set; } = "";
        
        public Guid OwnerId { get; set; } // Will be null in the user-request, but not when stored in the database.
        public DateTime TimeCreated { get; set; }
        public List<ForumReply> Replies { get; set; }
        public uint Views { get; set; }
        public List<Guid> Likes { get; set; }
        public bool IsLocked { get; set; }

        public ForumData() {
            Id = Guid.NewGuid().ToString();
            Replies = new List<ForumReply>();
            TimeCreated = DateTime.Now;
            Views = 0;
            Likes = new List<Guid>();
        }
    }
}