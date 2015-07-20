using System;
using System.Collections.Generic;
using MongoBasic.Core;

namespace MongoBasic.Tests.TestHelper
{
    public class SessionFactoryBuilder
    {
        public static readonly IMongoSessionFactory MongoSessionFactory;

        static SessionFactoryBuilder()
        {
            var mongoSettings = new MongoSessionFactoryConfig
            {
                Server = TestConstants.Server,
                Port = TestConstants.Port,
                Database = TestConstants.Database,
                User = TestConstants.User,
                Password = TestConstants.Password,
                Collections = new Dictionary<Type, string>
                {
                    {typeof(Person), "Persons"},
                    {typeof(Vehicle), "Vehicles"}
                }
            };
            IMongoSessionFactory mongoSessionFactory = new TestSessionMongoFactory(mongoSettings);
            MongoSessionFactory = mongoSessionFactory;
        }
    }
}
