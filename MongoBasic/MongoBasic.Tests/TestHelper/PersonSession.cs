using MongoBasic.Core;
using MongoDB.Driver;

namespace MongoBasic.Tests.TestHelper
{
    public class PersonSession:AbstractMongoSession
    {
        public PersonSession(MongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }

        protected override void DefineClassMaps()
        {
            AddClassMap(new PersonMap());
        }

        protected override void DefineCollections()
        {
            AddCollection(typeof(Person), "Persons");
        }
    }
}
