using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace MongoBasic.Core
{
    public abstract class AbstractMongoSessionFactory:IMongoSessionFactory
    {
        private readonly MongoSessionFactoryConfig _configuration;
        private readonly MongoClientSettings _mongoClientSettings;
        private IList<IMongoClassMap> _classMaps; 

        protected AbstractMongoSessionFactory(MongoSessionFactoryConfig configuration)
        {
            if (configuration == null)
            {
                throw new MongoBasicException.ConfigurationException("Can not build session factory. The argument 'configuration' is null.");
            }
            if (String.IsNullOrEmpty(configuration.Server))
            {
                throw new MongoBasicException.ConfigurationException("Can not build session factory. The argument 'configuration.server' is null or empty.");
            }
            if (String.IsNullOrEmpty(configuration.Database))
            {
                throw new MongoBasicException.ConfigurationException("Can not build session factory. The argument 'configuration.database' is null or empty.");
            }
            _classMaps = new List<IMongoClassMap>();
            _mongoClientSettings = new MongoClientSettings { Server = new MongoServerAddress(configuration.Server, configuration.Port) };
            ConfigureAuth(configuration);
            _configuration = configuration;

            var mongoClient = new MongoClient(_mongoClientSettings);
            var mongoDatabase = mongoClient.GetServer().GetDatabase(_configuration.Database);
            DefineClassMaps(mongoDatabase);
            DefineIndexes(mongoDatabase);
        }

        protected abstract void DefineClassMaps(MongoDatabase mongoDatabase);
        protected abstract void DefineIndexes(MongoDatabase mongoDatabase);

        /// <summary>
        /// Opens a new Mongo Session instance against a Mongo dataabse defined in the session
        /// factory settings.
        /// </summary>
        public IMongoSession OpenSession()
        {
            if (_mongoClientSettings == null)
            {
                throw new MongoBasicException.ConfigurationException("Can not build mongo session. Mongo client settings have not been defined. Have you called SetMongoClientSettings before building a Mongo session?");
            }

            var mongoClient = new MongoClient(_mongoClientSettings);
            MongoDatabase database = mongoClient.GetServer().GetDatabase(_configuration.Database);
            IMongoSession session = (IMongoSession)Activator.CreateInstance(typeof(MongoSession), database, _configuration.Collections);
            return session;
        }

        protected void AddClassMap(IMongoClassMap classMap)
        {
            if (classMap == null)
            {
                throw new Exception("The class map can not be null");
            }
            _classMaps.Add(classMap);
        }

        private void ConfigureAuth(MongoSessionFactoryConfig configuration)
        {
            if (!string.IsNullOrEmpty(configuration.User))
            {
                switch (configuration.CredentialType)
                {
                    case MongoCredentialType.MONGODB_CR:
                    {
                        MongoCredential credentials = MongoCredential.CreateMongoCRCredential(configuration.Database, configuration.User,
                            configuration.Password);
                        _mongoClientSettings.Credentials = new List<MongoCredential>() {credentials};
                        break;
                    }
                    case MongoCredentialType.SCRAM_SHA_1:
                    {
                        MongoCredential credentials = MongoCredential.CreateScramSha1Credential(configuration.Database, configuration.User,
                            configuration.Password);
                        _mongoClientSettings.Credentials = new List<MongoCredential>() {credentials};
                        break;
                    }
                    case MongoCredentialType.PLAIN:
                    {
                        MongoCredential credentials = MongoCredential.CreatePlainCredential(configuration.Database, configuration.User,
                            configuration.Password);
                        _mongoClientSettings.Credentials = new List<MongoCredential>() {credentials};
                        break;
                    }
                }
            }
        }
    }
}
