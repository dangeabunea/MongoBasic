using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoBasic.Core
{
    /// <summary>
    /// The abstract mongo session is the base class that needs to be ovveriden by concrete, specific
    /// implementaitons.
    /// </summary>
    public sealed class MongoSession : IMongoSession
    {
        private readonly MongoDatabase _database;
        private readonly IDictionary<Type, string> _registeredCollections;


        public MongoSession(MongoDatabase mongoDatabase, IDictionary<Type, string> collections)
        {
            _database = mongoDatabase;
            _registeredCollections = collections;
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

        public TEntity Get<TEntity>(object id)
        {
            var collection = GetCollection<TEntity>();
            Type entityIdType = typeof(TEntity).GetProperty("Id").PropertyType;

            IMongoQuery findUniqueQuery;
            if (entityIdType == typeof(String))
            {
                findUniqueQuery = MongoDB.Driver.Builders.Query.EQ("_id", id.ToString());
            }
            else if (entityIdType == typeof(ObjectId))
            {
                findUniqueQuery = MongoDB.Driver.Builders.Query.EQ("_id", (ObjectId)id);
            }
            else if (entityIdType == typeof(long))
            {
                findUniqueQuery = MongoDB.Driver.Builders.Query.EQ("_id", (long)id);
            }
            else if (entityIdType == typeof(int))
            {
                findUniqueQuery = MongoDB.Driver.Builders.Query.EQ("_id", (int)id);
            }
            else
            {
                throw new Exception("Id is not of known types;");
            }

            var result = collection.Find(findUniqueQuery);
            return result.SingleOrDefault();
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
            var collection = GetCollection<TEntity>();
            object entityIdValue = entity.GetType().GetProperty("Id").GetValue(entity, null);
            Type entityIdType = entity.GetType().GetProperty("Id").PropertyType;

            IMongoQuery deleteQuery;
            if (entityIdType == typeof(String))
            {
                deleteQuery = MongoDB.Driver.Builders.Query.EQ("_id", entityIdValue.ToString());
            }
            else if (entityIdType == typeof(ObjectId))
            {
                deleteQuery = MongoDB.Driver.Builders.Query.EQ("_id", (ObjectId)entityIdValue);
            }
            else if (entityIdType == typeof(long))
            {
                deleteQuery = MongoDB.Driver.Builders.Query.EQ("_id", (long)entityIdValue);
            }
            else if (entityIdType == typeof(int))
            {
                deleteQuery = MongoDB.Driver.Builders.Query.EQ("_id", (int)entityIdValue);
            }
            else
            {
                throw new Exception("Id is not of known types;");
            }

            collection.Remove(deleteQuery);
        }

        public void DeleteAll<TEntity>()
        {
            var collection = GetCollection<TEntity>();
            collection.RemoveAll();
        }

        public void DeleteAllCollections()
        {
            foreach (var collectionName in _database.GetCollectionNames())
            {
                if (collectionName.Contains("system."))
                {
                    //we are not allowed to remove or clear system collections
                    continue;
                }
                _database.DropCollection(collectionName);
            }
        }

        public int Count<TEntity>()
        {
            return Query<TEntity>().Count();
        }

        public DatabaseStatsResult Status()
        {
            return _database.GetStats();
        }


        private MongoCollection<TEntity> GetCollection<TEntity>()
        {
            try
            {
                var collectionName = _registeredCollections[typeof(TEntity)];
                return _database.GetCollection<TEntity>(collectionName);
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
