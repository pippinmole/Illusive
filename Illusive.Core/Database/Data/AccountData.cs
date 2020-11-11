using System;
using Illusive.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Illusive.Models {
    public class AccountData {
        
        [BsonId]
        public string Id { get; set; }
        
        [BsonElement("Name"), UnicodeOnly]
        public string AccountName { get; set; }
        
        public string Email { get; set; }
        
        public uint Age { get; set; }
        
        public string HashedPassword { get; set; }
        
        public string ProfilePicture { get; set; }
        
        public string Bio { get; set; }
        
        public DateTime AccountCreated { get; set; }
        
        public AccountData(string id, string accountName, string email, uint age, string hashedPassword) {
            this.Id = id;
            this.AccountName = accountName;
            this.Email = email;
            this.Age = age;
            this.HashedPassword = hashedPassword;
            this.ProfilePicture = "https://t4.ftcdn.net/jpg/00/64/67/63/240_F_64676383_LdbmhiNM6Ypzb3FM4PPuFP9rHe7ri8Ju.jpg";
            this.Bio = "I'm new here!";
            this.AccountCreated = DateTime.Now;
        }
    }
}