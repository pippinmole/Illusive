using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Illusive.Illusive.Database.Models {
    public class AccountData {
        
        [BsonId]
        public string Id { get; set; }
        
        [BsonElement("Name")]
        public string AccountName { get; set; }
        
        public string Email { get; set; }
        
        public uint Age { get; set; }

        public AccountData(string id, string accountName, string email, uint age) {
            this.Id = id;
            this.AccountName = accountName;
            this.Email = email;
            this.Age = age;
        }
    }
}