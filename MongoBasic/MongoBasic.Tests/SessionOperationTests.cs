using System.Collections.Generic;
using System.Linq;
using MongoBasic.Core;
using MongoBasic.Tests.TestHelper;
using Xunit;

namespace MongoBasic.Tests
{
    public class SessionOperationTests
    {
        private readonly IMongoSession _session;

        public SessionOperationTests()
        {
            var mongoSettings = new MongoSessionFactoryConfig
            {
                Server = TestConstants.Server,
                Port = TestConstants.Port,
                Database = TestConstants.Database,
                User = TestConstants.User,
                Password = TestConstants.Password
            };
            IMongoSessionFactory mongoSessionFactory = new MongoSessionFactory(mongoSettings);
            _session = mongoSessionFactory.OpenSession<PersonSession>();
            
            //delete all collections prior to running each test
            _session.DeleteAllCollections();
        }

        [Fact]
        public void The_save_method_should_insert_one_typed_object_into_the_db()
        {
            //arrange
            var person = new Person {Name = "Adrian", Age = 23};

            //act
            _session.Save(person);

            //assert
            Assert.Equal(1, _session.Count<Person>());
        }

        [Fact]
        public void The_save_batch_method_should_insert_batch_of_multiple_elements_into_the_db()
        {
            //arrange
            var persons = new List<Person>();
            var person1 = new Person { Name = "Adrian", Age = 23 };
            var person2 = new Person { Name = "Anna", Age = 33 };
            persons.Add(person1);
            persons.Add(person2);

            //act
            _session.SaveBatch(persons);

            //assert
            Assert.Equal(persons.Count, _session.Count<Person>());
        }

        [Fact]
        public void The_query_method_should_return_linq_queryable_list()
        {
            //arrange
            var persons = new List<Person>();
            var person1 = new Person { Name = "Adrian", Age = 23 };
            var person2 = new Person { Name = "Anna", Age = 33 };
            var person3 = new Person { Name = "Brian", Age = 43 };
            persons.Add(person1);
            persons.Add(person2);
            persons.Add(person3);

            _session.SaveBatch(persons);

            //act
            IList<Person> personsOlderThanThirty = 
                _session.Query<Person>().Where(x => x.Age > 30).ToList();

            //assert
            Assert.Equal(2, personsOlderThanThirty.Count);
        }
    }
}
