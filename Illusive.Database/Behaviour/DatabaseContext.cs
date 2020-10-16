using System;
using Illusive.Illusive.Database.Interfaces;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Illusive.Illusive.Database.Behaviour {
    public class DatabaseContext : IDatabaseContext {
        
        public IMongoClient Client { get; }
        public string ConnectionString { get; }

        public DatabaseContext() {
            Console.WriteLine($"Couldn't find an IConfiguration dependency!");
        }
        
        public DatabaseContext(IConfiguration configuration) {
            this.ConnectionString = configuration.GetConnectionString("MongoDBConnectionString");
            this.Client = new MongoClient(this.ConnectionString);
            
            Console.WriteLine($"Creating database context: ConnString: {this.ConnectionString}");
        }
    }
}