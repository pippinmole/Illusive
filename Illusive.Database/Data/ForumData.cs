﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Illusive.Illusive.Database.Models {
    public class ForumData {

        [BsonId] public string Id { get; set; }

        [Required] public string Title { get; set; }

        [Required] public string Content { get; set; }
        
        public string OwnerId { get; set; }

        public DateTime TimeCreated { get; set;  }

        public List<ForumReply> Replies { get; set; }
        public bool IsMarkdown { get; set; }
        public uint Views { get; set; }

        public ForumData() {
            this.Id = Guid.NewGuid().ToString();
            this.Replies = new List<ForumReply>();
            this.TimeCreated = DateTime.Now;
            this.Views = 0;
        }
    }
}