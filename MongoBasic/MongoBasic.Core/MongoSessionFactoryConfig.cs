using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace MongoBasic.Core
{
    /// <summary>
    /// Mongo session factory configuration object.
    /// </summary>
    public sealed class MongoSessionFactoryConfig
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; set; }
        public MongoCredentialType CredentialType { get; set; }
        public IDictionary<Type, string> Collections { get; set; }

        public MongoSessionFactoryConfig()
        {
            Server = "localhost";
            Port = 25017;
            CredentialType = MongoCredentialType.MONGODB_CR;
            Collections = new Dictionary<Type, string>();
            ConnectionString = null;
        }

        public MongoSessionFactoryConfig(string mongoConnectionString)
        {
            Collections = new Dictionary<Type, string>();
            CredentialType = MongoCredentialType.MONGODB_CR;
            ConnectionString = mongoConnectionString;
            MongoUrl mongoUrl = new MongoUrl(mongoConnectionString);
            Server = mongoUrl.Server.Host;
            Port = mongoUrl.Server.Port;
            Database = mongoUrl.DatabaseName;
            User = mongoUrl.Username;
            Password = mongoUrl.Password;
        }

        public bool PerformAuth()
        {
            return !string.IsNullOrEmpty(User);
        }
    }
}
