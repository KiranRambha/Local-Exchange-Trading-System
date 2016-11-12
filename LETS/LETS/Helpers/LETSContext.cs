using LETS.Models;
using LETS.Properties;
using MongoDB.Driver;

namespace LETS.Helpers
{
    public class LETSContext
    {
        public IMongoDatabase Database;

        public LETSContext()
        {
            var connectionString = string.Concat(Settings.Default.LETSConnectionString, "/", Settings.Default.LETSDatabaseName);
            var client = new MongoClient(connectionString);
            Database = client.GetDatabase(Settings.Default.LETSDatabaseName);
        }
        
        public IMongoCollection<RegisterUserViewModel> RegisteredUsers => Database.GetCollection<RegisterUserViewModel>("registeredUsers");

        public IMongoCollection<LetsTradingDetails> LetsTradingDetails => Database.GetCollection<LetsTradingDetails>("letsTradingDetails");
    }
}