using MongoBasic.Core;
using MongoBasic.Core.IdGeneration;
using MongoDB.Driver;

namespace MongoBasic.Tests.TestHelper
{
    public class TestSessionMongoFactory : AbstractMongoSessionFactory
    {
        public TestSessionMongoFactory(MongoSessionFactoryConfig mongoSettings) : base(mongoSettings)
        {
        }

        protected override void AfterSessionFactoryInitialized(MongoDatabase mongoDatabase)
        {
            AddClassMap(new PersonMap());
            AddClassMap(new VehicleMap(new CollectionAutoIncrementIdGenerator<long>(mongoDatabase, "Vehicles")));
        }
    }
}