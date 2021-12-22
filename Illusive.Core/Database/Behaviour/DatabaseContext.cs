using Illusive.Illusive.Database.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Illusive.Database {
    public class DatabaseContext : IDatabaseContext {
        private readonly ILogger<DatabaseContext> _logger;

        public IMongoClient Client { get; }
        public string ConnectionString { get; }
        
        public DatabaseContext(IConfiguration configuration, ILogger<DatabaseContext> logger) {
            _logger = logger;
            
            _logger.LogInformation("Connecting to database...");
            
            ConnectionString = configuration.GetConnectionString("DatabaseConnectionString");
            Client = new MongoClient(ConnectionString);

            _logger.LogInformation("Connected to database...");
        }
        
        public IMongoDatabase GetDatabase(string name) => Client.GetDatabase(name);
    }
}