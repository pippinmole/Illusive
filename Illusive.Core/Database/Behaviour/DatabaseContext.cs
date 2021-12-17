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
            this._logger = logger;
            
            this._logger.LogInformation("Connecting to database...");
            
            this.ConnectionString = configuration.GetConnectionString("DatabaseConnectionString");
            this.Client = new MongoClient(this.ConnectionString);

            this._logger.LogInformation("Connected to database...");
        }
        
        public IMongoDatabase GetDatabase(string name) => this.Client.GetDatabase(name);
    }
}