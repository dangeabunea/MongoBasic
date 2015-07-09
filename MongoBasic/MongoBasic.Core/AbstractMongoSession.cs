using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;

namespace MongoBasic.Core
{
    /// <summary>
    /// The abstract mongo session is the base class that needs to be ovveriden by concrete, specific
    /// implementaitons.
    /// </summary>
    public abstract class AbstractMongoSession : IMongoSession
    {
        protected readonly MongoDatabase Database;
        protected readonly IDictionary<Type, string> RegisteredCollections;
        protected readonly IList<IMongoClassMap> RegisteredClassMaps;

        protected AbstractMongoSession(MongoDatabase mongoDatabase)
        {
            Database = mongoDatabase;
            RegisteredClassMaps = new List<IMongoClassMap>();
            RegisteredCollections = new Dictionary<Type, string>();
            
            //will use concrete type implementations
            DefineCollections();
            DefineClassMaps();
            DefineIndexes();
        }

        public IQueryable<TEntity> Query<TEntity>()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Query<TEntity>(string[] fields)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntitySubType> QuerySubType<TEntity, TEntitySubType>()
        {
            throw new NotImplementedException();
        }

        public void Save<TEntity>(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void SaveBatch<TEntity>(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete<TEntity>(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void DeleteAll<TEntity>()
        {
            throw new NotImplementedException();
        }

        public void DeleteAllCollections()
        {
            throw new NotImplementedException();
        }

        public int Count<TEntity>()
        {
            throw new NotImplementedException();
        }

        protected abstract void DefineClassMaps();

        protected abstract void DefineCollections();

        protected virtual void DefineIndexes()
        {
        }

        protected MongoCollection<TEntity> GetCollection<TEntity>()
        {
            try
            {
                var collectionName = RegisteredCollections[typeof(TEntity)];
                return Database.GetCollection<TEntity>(collectionName);
            }
            catch (Exception ex)
            {
                String message = 
                    "Could not find a registered collection for type " + 
                    typeof (TEntity).Name + 
                    ". Did you forget to add this entity type to the regitered collections?";
                throw new MongoBasicException.ConfigurationException(message);
            }
        }
    }
}
