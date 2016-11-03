using LETS.Models;
using LETS.Properties;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LETS.Helpers
{
    public class LETSContext
    {
        public IMongoDatabase Database;

        public LETSContext()
        {
            string connectionString = string.Concat(Settings.Default.LETSConnectionString, "/", Settings.Default.LETSDatabaseName);
            var client = new MongoClient(connectionString);
            Database = client.GetDatabase(Settings.Default.LETSDatabaseName);
        }
        
        public IMongoCollection<RegisterUserViewModel> RegisteredUsers => Database.GetCollection<RegisterUserViewModel>("registeredUsers");
    }
}