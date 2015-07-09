using MongoBasic.Core;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MongoBasic.Tests.TestHelper
{
    public class PersonMap : MongoClassMap<Person>
    {
        protected override void MapEntity(BsonClassMap<Person> cm)
        {
            cm.MapIdField(x => x.Id).SetIdGenerator(new ObjectIdGenerator());
            cm.MapProperty(x => x.Name);
            cm.MapProperty(x => x.Age);
        }
    }
}
