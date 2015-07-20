using System;
using System.Collections.Generic;
using System.Linq;
using MongoBasic.Core;
using MongoBasic.Tests.TestHelper;
using Xunit;

namespace MongoBasic.Tests
{
    public class SessionTests : IDisposable
    {
        private IMongoSession _session;

        public SessionTests()
        {
            _session = SessionFactoryBuilder.MongoSessionFactory.OpenSession();
            _session.DeleteAllCollections();
        }

        public void Dispose()
        {
            _session.DeleteAllCollections();
            _session = null;
        }

        [Fact]
        public void The_save_method_should_insert_one_typed_object_into_the_db()
        {
            //arrange
            var person = new Person { Name = "Adrian", Age = 23 };

            //act
            _session.Save(person);

            //assert
            Assert.Equal(1, _session.Count<Person>());
        }

        [Fact]
        public void The_save_batch_method_with_bson_id_should_insert_batch_of_multiple_elements_into_the_db()
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
        public void The_save_batch_method_with_autoincrement_id_should_insert_multiple_elements_into_the_db()
        {
            //arrange
            var vehicles = new List<Vehicle>();
            var vehicle1 = new Vehicle { Make = "Skoda", Model = "Octavia" };
            var vehicle2 = new Vehicle { Make = "Renault", Model = "Clio" };
            var vehicle3 = new Vehicle { Make = "Ford", Model = "Mondeo" };
            vehicles.Add(vehicle1);
            vehicles.Add(vehicle2);
            vehicles.Add(vehicle3);

            //act
            _session.SaveBatch(vehicles);

            //assert
            Assert.Equal(3, _session.Count<Vehicle>());
        }

        [Fact]
        public void The_delete_method_with_bson_id_should_delete_entity()
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
            _session.Delete(person2);
            _session.Delete(person1);

            //assert
            Assert.Equal(1, _session.Count<Person>());
        }

        [Fact]
        public void The_delete_method_with_autoincrement_id_should_delete_entity()
        {
            //arrange
            var vehicles = new List<Vehicle>();
            var vehicle1 = new Vehicle {Make = "Skoda", Model = "Octavia"};
            var vehicle2 = new Vehicle { Make = "Renault", Model = "Clio" };
            var vehicle3 = new Vehicle { Make = "Ford", Model = "Mondeo" };
            vehicles.Add(vehicle1);
            vehicles.Add(vehicle2);
            vehicles.Add(vehicle3);
            _session.SaveBatch(vehicles);

            //act
            _session.Delete(vehicle2);
            _session.Delete(vehicle1);

            //assert
            Assert.Equal(1, _session.Count<Vehicle>());
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

        [Fact]
        public void The_update_method_with_bson_id_should_update_entity_in_the_database()
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
            person2.Name = "Dan";
            person2.Age = 50;
            _session.Save(person2);

            //assert
            var person2FromTheDb = _session.Query<Person>().Single(x => x.Id == person2.Id);
            Assert.Equal("Dan", person2FromTheDb.Name);
            Assert.Equal(50, person2FromTheDb.Age);
        }

        [Fact]
        public void The_update_method_with_autoincrement_id_should_update_entity_in_the_database()
        {
            //arrange
            var vehicles = new List<Vehicle>();
            var vehicle1 = new Vehicle { Make = "Skoda", Model = "Octavia" };
            var vehicle2 = new Vehicle { Make = "Renault", Model = "Clio" };
            var vehicle3 = new Vehicle { Make = "Ford", Model = "Mondeo" };
            vehicles.Add(vehicle1);
            vehicles.Add(vehicle2);
            vehicles.Add(vehicle3);
            _session.SaveBatch(vehicles);

            //act
            vehicle2.Make = "Peugeot";
            vehicle2.Model = "206";
            _session.Save(vehicle2);

            //assert
            var vehicle2FromTheDb = _session.Query<Vehicle>().Single(x => x.Id == vehicle2.Id);
            Assert.Equal("Peugeot", vehicle2FromTheDb.Make);
            Assert.Equal("206", vehicle2FromTheDb.Model);

        }

        [Fact]
        public void The_get_method_with_bson_id_should_return_entity_by_id()
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
            var personById = _session.Get<Person>(person2.Id);

            //assert
            Assert.Equal("Anna", personById.Name);
            Assert.Equal(33, personById.Age);
        }

        [Fact]
        public void The_get_method_with_autoincrement_id_should_return_entity_by_id()
        {
            //arrange
            var vehicles = new List<Vehicle>();
            var vehicle1 = new Vehicle { Make = "Skoda", Model = "Octavia" };
            var vehicle2 = new Vehicle { Make = "Renault", Model = "Clio" };
            var vehicle3 = new Vehicle { Make = "Ford", Model = "Mondeo" };
            vehicles.Add(vehicle1);
            vehicles.Add(vehicle2);
            vehicles.Add(vehicle3);
            _session.SaveBatch(vehicles);

            //act
            var vehicleById = _session.Get<Vehicle>(vehicle2.Id);

            //assert
            Assert.Equal("Renault", vehicleById.Make);
            Assert.Equal("Clio", vehicleById.Model);
        }

        [Fact]
        public void The_get_method_should_return_null_if_no_object_is_found_by_id()
        {
            //arrange
            var vehicles = new List<Vehicle>();
            var vehicle1 = new Vehicle { Make = "Skoda", Model = "Octavia" };
            var vehicle2 = new Vehicle { Make = "Renault", Model = "Clio" };
            var vehicle3 = new Vehicle { Make = "Ford", Model = "Mondeo" };
            vehicles.Add(vehicle1);
            vehicles.Add(vehicle2);
            vehicles.Add(vehicle3);
            _session.SaveBatch(vehicles);

            //act
            long someIdNotInDb = 234;
            var vehicleById = _session.Get<Vehicle>(someIdNotInDb);

            //assert
            Assert.Null(vehicleById);
        }

        [Fact]
        public void The_delete_all_method_should_remove_all_entities_from_collection()
        {
            //arrange
            var vehicles = new List<Vehicle>();
            var vehicle1 = new Vehicle { Make = "Skoda", Model = "Octavia" };
            var vehicle2 = new Vehicle { Make = "Renault", Model = "Clio" };
            var vehicle3 = new Vehicle { Make = "Ford", Model = "Mondeo" };
            vehicles.Add(vehicle1);
            vehicles.Add(vehicle2);
            vehicles.Add(vehicle3);
            _session.SaveBatch(vehicles);

            //act
            _session.DeleteAll<Vehicle>();

            //assert
            Assert.Empty(_session.Query<Vehicle>().ToList());
        }
    }
}
