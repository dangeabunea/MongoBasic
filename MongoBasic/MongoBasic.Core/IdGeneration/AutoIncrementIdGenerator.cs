using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace MongoBasic.Core.IdGeneration
{
    public class AutoIncrementIdGenerator<T> : IIdGenerator
    {
        private const int CAPACITY = 20;
        private readonly int _capacity;
        private readonly object _generatorLock = new object();
        private IDictionary<string, AutoIncrementIdImplementation> _keyGeneratorsByTag = new Dictionary<string, AutoIncrementIdImplementation>();
        private readonly MongoDatabase _mongoDatabase;
        private readonly string _collectionName;

        public AutoIncrementIdGenerator(MongoDatabase db, string collectionName)
            : this(CAPACITY, db, collectionName)
        {
        }

        public AutoIncrementIdGenerator(int capacity, MongoDatabase mongoDatabase, string collectionName)
        {
            this._capacity = capacity;
            this._mongoDatabase = mongoDatabase;
            this._collectionName = collectionName;
        }

        public object GenerateId(object container, object document)
        {
            AutoIncrementIdImplementation value;
            Type parameterType = typeof(T);

            lock (_generatorLock)
            {
                if (_keyGeneratorsByTag.TryGetValue(_collectionName, out value))
                {
                    if (parameterType == typeof(long)) return value.GenerateId(_collectionName);
                    else if (parameterType == typeof(int)) return unchecked((int)value.GenerateId(_collectionName));
                    else return null;
                }

                value = new AutoIncrementIdImplementation(_capacity, _mongoDatabase);
                _keyGeneratorsByTag = new Dictionary<string, AutoIncrementIdImplementation>(_keyGeneratorsByTag)
                {
                    {_collectionName, value}
                };
            }

            var id = value.GenerateId(_collectionName);

            if (parameterType == typeof(long)) return id;
            else if (parameterType == typeof(int)) return unchecked((int)id);
            else return null;
        }

        public bool IsEmpty(object id)
        {
            if (id == null) return true;
            return ((T)id).Equals(default(T));
        }
    }
}
