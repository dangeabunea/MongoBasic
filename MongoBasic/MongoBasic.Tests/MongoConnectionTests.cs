using System;
using System.Configuration;
using MongoBasic.Core;
using MongoBasic.Core.Abstract;
using MongoBasic.Tests.TestHelper;
using MongoDB.Driver;
using Xunit;

namespace MongoBasic.Tests
{
    public class MongoConnectionTests
    {
        [Fact]
        public void With_client_settings_should_establish_connection_to_database()
        {
            //arrange
            var mongoSettings = new MongoSessionFactoryConfig
            {
                Server = ConfigurationManager.AppSettings["server"],
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]),
                Database = ConfigurationManager.AppSettings["database"],
                User = ConfigurationManager.AppSettings["user"],
                Password = ConfigurationManager.AppSettings["password"],
            };
            IMongoSessionFactory mongoSessionFactory = new TestSessionMongoFactory(mongoSettings);

            //act
            IMongoSession mongoSession = mongoSessionFactory.OpenSession();

            //assert
            Assert.True(mongoSession.Status().ErrorMessage == null, "Mongo session should have been open.");
        }

        [Fact]
        public void With_uri_should_establish_connection_to_database()
        {
            //arrange
            var mongoSettings = new MongoSessionFactoryConfig(ConfigurationManager.AppSettings["mongoUri"]);
            IMongoSessionFactory mongoSessionFactory = new TestSessionMongoFactory(mongoSettings);

            //act
            IMongoSession mongoSession = mongoSessionFactory.OpenSession();

            //assert
            Assert.True(mongoSession.Status().ErrorMessage == null, "Mongo session should have been open.");
        }


        [Fact]
        public void Whith_connection_config_invalid_should_throw_mongo_connection_exception()
        {
            //arrange
            int invalidPort = 5555;
            var mongoSettings = new MongoSessionFactoryConfig
            {
                Server = ConfigurationManager.AppSettings["server"],
                Port = invalidPort,
                Database = ConfigurationManager.AppSettings["database"],
                User = ConfigurationManager.AppSettings["user"],
                Password = ConfigurationManager.AppSettings["password"],
            };
            IMongoSessionFactory mongoSessionFactory = new TestSessionMongoFactory(mongoSettings);

            //act
            IMongoSession mongoSession = mongoSessionFactory.OpenSession();

            //assert
            Assert.Throws<MongoConnectionException>(() => { var status = mongoSession.Status(); });
        }

        [Fact]
        public void Whith_invalid_credentials_should_throw_mongo_connection_exception()
        {
            //arrange
            string invalidPass = "fdhfdj";
            var mongoSettings = new MongoSessionFactoryConfig
            {
                Server = ConfigurationManager.AppSettings["server"],
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]),
                Database = ConfigurationManager.AppSettings["database"],
                User = ConfigurationManager.AppSettings["user"],
                Password = invalidPass,
            };
            IMongoSessionFactory mongoSessionFactory = new TestSessionMongoFactory(mongoSettings);

            //act
            IMongoSession mongoSession = mongoSessionFactory.OpenSession();

            //assert
            Assert.Throws<MongoConnectionException>(() => { var status = mongoSession.Status(); });
        }
    }
}
