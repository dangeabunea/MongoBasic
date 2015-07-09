using MongoDB.Bson.Serialization;

namespace MongoBasic.Core
{
    public abstract class MongoClassMap<T> : IMongoClassMap
    {
        protected MongoClassMap()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            {
                BsonClassMap.RegisterClassMap<T>(MapEntity);
            }
        }

        /// <summary>
        /// Can be ovveridden by concrete map implementations
        /// </summary>
        protected virtual void MapEntity(BsonClassMap<T> cm)
        {
            cm.AutoMap();
        }
    }
}
