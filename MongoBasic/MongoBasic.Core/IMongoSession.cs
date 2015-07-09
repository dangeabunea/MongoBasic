using System.Collections.Generic;
using System.Linq;

namespace MongoBasic.Core
{
    public interface IMongoSession
    {
        IQueryable<TEntity> Query<TEntity>();
        IQueryable<TEntity> Query<TEntity>(string[] fields);
        IQueryable<TEntitySubType> QuerySubType<TEntity, TEntitySubType>();
        void Save<TEntity>(TEntity entity);
        void SaveBatch<TEntity>(IEnumerable<TEntity> entities);
        void Delete<TEntity>(TEntity entity);
        void DeleteAll<TEntity>();
        void DeleteAllCollections();
        int Count<TEntity>();
    }
}