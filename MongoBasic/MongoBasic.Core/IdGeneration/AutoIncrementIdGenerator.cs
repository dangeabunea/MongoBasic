using System;
using System.Threading;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MongoBasic.Core.IdGeneration
{
    public class AutoIncrementIdGenerator : IDisposable
    {
        private readonly long _capacity;
        private readonly ReaderWriterLockSlim _lockSlim = new ReaderWriterLockSlim();
        private long _currentHi;
        private long _currentLo;
        private readonly MongoDatabase _mongo;

        public long CurrentHi { get { return _currentHi; } }

        public AutoIncrementIdGenerator(long capacity, MongoDatabase mongo)
        {
            _currentHi = 0;
            _capacity = capacity;
            _mongo = mongo;
            _currentLo = capacity + 1;
        }

        public long GenerateId(string collectionName)
        {
            _lockSlim.EnterUpgradeableReadLock();
            long incrementedCurrentLow = Interlocked.Increment(ref _currentLo);

            if (incrementedCurrentLow > _capacity)
            {
                _lockSlim.EnterWriteLock();
                if (Thread.VolatileRead(ref _currentLo) > _capacity)
                {
                    _currentHi = GetNextHi(collectionName, _mongo);
                    _currentLo = 1;
                    incrementedCurrentLow = 1;
                }
                _lockSlim.ExitWriteLock();
            }

            _lockSlim.ExitUpgradeableReadLock();
            return (_currentHi - 1) * _capacity + (incrementedCurrentLow);
        }

        public void Dispose()
        {
            _lockSlim.Dispose();
        }


        private long GetNextHi(string collectionName, MongoDatabase database)
        {
            while (true)
            {
                try
                {
                    var query = Query.EQ("_id", collectionName);
                    var sortBy = SortBy.Descending("_id");
                    var update = Update.Inc("ServerValue", 1);

                    var hiLoKey = database
                        .GetCollection<AutoIncrementKey>("Ids")
                        .FindAndModify(new FindAndModifyArgs()
                        {
                            Query = query,
                            SortBy = sortBy,
                            Update = update,
                            Upsert = true

                        })
                        .GetModifiedDocumentAs<AutoIncrementKey>();

                    if (hiLoKey == null)
                    {
                        database.GetCollection<AutoIncrementKey>("Ids")
                                .Insert(new AutoIncrementKey { Collection = collectionName, ServerValue = 1 });
                        return 1;
                    }

                    var newHi = hiLoKey.ServerValue;
                    return newHi;
                }
                catch (MongoException ex)
                {
                    if (!ex.Message.Contains("duplicate key"))
                        throw;
                }
            }
        }

        private class AutoIncrementKey
        {
            [BsonId]
            public string Collection;

            public int ServerValue;
        }
    }
}
