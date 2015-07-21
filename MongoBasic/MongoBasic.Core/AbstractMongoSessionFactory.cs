using System;
using System.Collections.Generic;
using MongoBasic.Core.Abstract;
using MongoDB.Driver;

namespace MongoBasic.Core
{
    /// <summary>
    /// Abstarct instance of a mongo session factory that provides most common behaviour. It needs to be inherited by a 
    /// concrete class where the 'AfterSessionFactoryInitialized' needs to be implemented.
    /// </summary>
    public abstract class AbstractMongoSessionFactory:IMongoSessionFactory
    {
        private readonly MongoSessionFactoryConfig _configuration;
        private readonly MongoClientSettings _mongoClientSettings;
        private readonly IList<IMongoClassMap> _classMaps;
        private readonly MongoDatabase _mongoDatabase;

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
            _configuration = configuration;
            ConfigureAuth(configuration);
            var mongoClient = new MongoClient(_mongoClientSettings);
            _mongoDatabase = mongoClient.GetServer().GetDatabase(_configuration.Database);

            AfterSessionFactoryInitialized(_mongoDatabase);
        }

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

        /// <summary>
        /// Add a class mapping that will be used throughout the lifecycle of the applicaiton
        /// </summary>
        protected void RegisterClassMap(IMongoClassMap classMap)
        {
            if (classMap == null)
            {
                throw new ArgumentNullException("classMap");
            }
            _classMaps.Add(classMap);
        }

        /// <summary>
        /// Get an instance to a mongo collection by the type of the entity
        /// </summary>
        /// <typeparam name="T">The type of the entity</typeparam>
        /// <returns></returns>
        protected MongoCollection GetCollection<T>()
        {
            try
            {
                string collectionName = _configuration.Collections[typeof (T)];
                var collection = _mongoDatabase.GetCollection<T>(collectionName);
                return collection;
            }
            catch (Exception ex)
            {
                throw new MongoBasicException.CollectionNotFoundException(typeof(T).Name, ex);
            }
        }

        /// <summary>
        /// Exptensibility point that is executed after the session factory has been initialized and a conneciton
        /// to the database has been done. It can be used to add class mappings, indexes or other custom logic
        /// </summary>
        /// <param name="mongoDatabase">An instance to the mongo databse</param>
        protected abstract void AfterSessionFactoryInitialized(MongoDatabase mongoDatabase);

        private void ConfigureAuth(MongoSessionFactoryConfig configuration)
        {
            if (configuration.PerformAuth())
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
