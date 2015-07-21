using System;
using System.Collections.Generic;
using System.Linq;
using MongoBasic.Core.Abstract;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MongoBasic.Core
{
    public sealed class MongoSession : IMongoSession
    {
        private readonly MongoDatabase _database;
        private readonly IDictionary<Type, string> _registeredCollections;


        public MongoSession(MongoDatabase mongoDatabase, IDictionary<Type, string> collections)
        {
            _database = mongoDatabase;
            _registeredCollections = collections;
        }

        /// <summary>
        /// Returns an IQueryable instance of a collection on which you can build lambda expressions for filtering, ordering,
        /// grouping, etc. All fields will be returned by the query.
        /// </summary>
        public IQueryable<TEntity> Query<TEntity>()
        {
            var collection = GetCollection<TEntity>();
            return collection.AsQueryable();
        }

        /// <summary>
        /// Returns an IQueryable instance of a collection on which you can build lambda expressions for filtering, ordering,
        /// grouping, etc. Only the fields in the parameter will be returned. On deserialization, if a field is not returned, 
        /// it will be deserialized with its default value.
        /// </summary>
        public IQueryable<TEntity> Query<TEntity>(string[] fields)
        {
            var collection = GetCollection<TEntity>().FindAllAs<TEntity>();
            collection.SetFields(fields);

            return collection.AsQueryable();
        }

        /// <summary>
        /// Returns an IQueryable instance of a sub-type collection on which you can build lambda expressions for filtering, ordering,
        /// grouping, etc. Only the subtype that is provided will be returned by the query. 
        /// </summary>
        public IQueryable<TEntitySubType> QuerySubType<TEntity, TEntitySubType>()
        {
            var collection = GetCollection<TEntity>();
            return collection.AsQueryable().OfType<TEntitySubType>();
        }

        /// <summary>
        /// Retireve an entity by id. Will return null if the query returns no results.
        /// </summary>
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

        /// <summary>
        /// Saves a document into it's corresponding collection. The entity needs to have an Id field. Based on the value of
        /// this field, the entity will be either inserted, or updated in the database.
        /// </summary>
        public void Save<TEntity>(TEntity entity)
        {
            var collection = GetCollection<TEntity>();
            collection.Save(entity);
        }

        /// <summary>
        /// Inserts multiple documents into a collection in one step.
        /// </summary>
        public void SaveBatch<TEntity>(IEnumerable<TEntity> entities)
        {
            var collection = GetCollection<TEntity>();
            collection.InsertBatch(entities);
        }

        /// <summary>
        /// Removes the document from a collection. The entity needs to have an Id field. This field is used to
        /// remove the document.
        /// </summary>
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
                throw new MongoBasicException.InvalidEntityException("Id is not of known types;");
            }

            collection.Remove(deleteQuery);
        }

        /// <summary>
        /// Removes all documents from a collection.
        /// </summary>
        public void DeleteAll<TEntity>()
        {
            var collection = GetCollection<TEntity>();
            collection.RemoveAll();
        }
        
        /// <summary>
        /// Drops all user defined collections from the database. Indexes and users are system collections and will not
        /// be affected.
        /// </summary>
        public void DropAllCollections()
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

        /// <summary>
        /// Returns the total number of documents in a collection.
        /// </summary>
        public int Count<TEntity>()
        {
            return Query<TEntity>().Count();
        }

        /// <summary>
        /// Returns the database connection status associated for this session.
        /// </summary>
        /// <returns></returns>
        public DatabaseStatsResult Status()
        {
            return _database.GetStats();
        }

        /// <summary>
        /// Returns a MongoDB collection object where you will have access to the native MongoDB
        /// driver functions. Suitable for advanced usage scenarios where the session facade is not
        /// enough.
        /// </summary>
        public MongoCollection<TEntity> GetCollection<TEntity>()
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
