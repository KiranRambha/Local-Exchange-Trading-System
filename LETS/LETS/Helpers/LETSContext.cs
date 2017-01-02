using LETS.Models;
using LETS.Properties;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

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
            ProfilePicturesBucket = new GridFSBucket(Database, new GridFSBucketOptions
            {
                BucketName = "ProfilePictures"
            });
        }
        
        public IMongoCollection<RegisterUserViewModel> RegisteredUsers => Database.GetCollection<RegisterUserViewModel>("registeredUsers");

        public IMongoCollection<LetsTradingDetails> LetsTradingDetails => Database.GetCollection<LetsTradingDetails>("letsTradingDetails");

        public GridFSBucket ProfilePicturesBucket { get; set; }
    }
}