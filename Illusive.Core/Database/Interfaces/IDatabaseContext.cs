using MongoDB.Driver;

namespace Illusive.Illusive.Database.Interfaces {
    public interface IDatabaseContext {
        
        IMongoClient Client { get; }
        string ConnectionString { get; }

        IMongoDatabase GetDatabase(string name);

    }
}