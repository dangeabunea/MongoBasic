using MongoBasic.Core;
using MongoDB.Driver;

namespace MongoBasic.Tests.TestHelper
{
    /// <summary>
    /// Simple test mongo session used to check connectivity against the database.
    /// </summary>
    public sealed class SimpleMongoSession:AbstractMongoSession
    {
        public SimpleMongoSession(MongoDatabase mongoDatabase) : base(mongoDatabase)
        {
            //intentionally left empty
        }

        protected override void DefineClassMaps()
        {
            //intentionally left empty
        }

        protected override void DefineCollections()
        {
            //intentionally left empty
        }
    }
}
