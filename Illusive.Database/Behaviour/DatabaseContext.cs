using System;
using Illusive.Illusive.Database.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Illusive.Illusive.Database.Behaviour {
    public class DatabaseContext : IDatabaseContext {
        
        public IMongoClient Client { get; }
        public string ConnectionString { get; }
        
        public DatabaseContext(IConfiguration configuration) {
            this.ConnectionString = configuration.GetConnectionString("AppConfig");
            this.Client = new MongoClient(this.ConnectionString);
        }
        
        public IMongoDatabase GetDatabase(string name) => this.Client.GetDatabase(name);
    }
}