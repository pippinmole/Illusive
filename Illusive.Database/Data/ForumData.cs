using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Illusive.Illusive.Database.Models {
    public class ForumData {

        [BsonId] public string Id { get; set; }

        [Required] public string Title { get; set; }

        [Required] public string Content { get; set; }

        public DateTime TimeCreated { get; set;  }

        public List<ForumReply> Replies { get; set; }

        public ForumData() {
            this.Replies = new List<ForumReply>();
        }
        public ForumData(string id, string title, string content) {
            this.Title = title;
            this.Content = content;
            this.Id = id;
            this.Replies = new List<ForumReply>();
            this.TimeCreated = DateTime.Now;
        }
    }
}