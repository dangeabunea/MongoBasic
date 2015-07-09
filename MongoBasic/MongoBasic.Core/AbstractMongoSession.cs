using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoBasic.Core
{
    /// <summary>
    /// The abstract mongo session is the base class that needs to be ovveriden by concrete, specific
    /// implementaitons.
    /// </summary>
    public abstract class AbstractMongoSession : IMongoSession
    {
        protected readonly MongoDatabase Database;
        private readonly IDictionary<Type, string> _registeredCollections;
        private readonly IList<IMongoClassMap> _registeredClassMaps;

        protected AbstractMongoSession(MongoDatabase mongoDatabase)
        {
            Database = mongoDatabase;
            _registeredClassMaps = new List<IMongoClassMap>();
            _registeredCollections = new Dictionary<Type, string>();
            
            //will use concrete type implementations
            DefineCollections();
            DefineClassMaps();
            DefineIndexes();
        }

        public IQueryable<TEntity> Query<TEntity>()
        {
            var collection = GetCollection<TEntity>();
            return collection.AsQueryable();
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
            var collection = GetCollection<TEntity>();
            collection.Save(entity);
        }

        public void SaveBatch<TEntity>(IEnumerable<TEntity> entities)
        {
            var collection = GetCollection<TEntity>();
            collection.InsertBatch(entities);
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
            foreach (var collectionName in Database.GetCollectionNames())
            {
                if (collectionName.Contains("system."))
                {
                    //we are not allowed to remove or clear system collections
                    continue;
                }
                Database.DropCollection(collectionName);
            }
        }

        public int Count<TEntity>()
        {
            return Query<TEntity>().Count();
        }

        public DatabaseStatsResult Status()
        {
            return Database.GetStats();
        }

        public void AddCollection(Type typeOfObject, String collectionName)
        {
            _registeredCollections.Add(new KeyValuePair<Type, string>(typeOfObject, collectionName));
        }

        public void AddClassMap(IMongoClassMap classMap)
        {
            _registeredClassMaps.Add(classMap);
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
                var collectionName = _registeredCollections[typeof(TEntity)];
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
