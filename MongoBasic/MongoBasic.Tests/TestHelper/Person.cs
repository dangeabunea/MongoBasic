using MongoDB.Bson;

namespace MongoBasic.Tests.TestHelper
{
    //Simple typed object to test basic mongo session operations
    public class Person
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
