using System;
using System.Collections.Generic;
using System.Configuration;
using MongoBasic.Core;
using MongoBasic.Core.Abstract;

namespace MongoBasic.Tests.TestHelper
{
    public class SessionFactoryBuilder
    {
        public static readonly IMongoSessionFactory MongoSessionFactory;

        static SessionFactoryBuilder()
        {
            var mongoSettings = new MongoSessionFactoryConfig
            {
                Server = ConfigurationManager.AppSettings["server"],
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]),
                Database = ConfigurationManager.AppSettings["database"],
                User = ConfigurationManager.AppSettings["user"],
                Password = ConfigurationManager.AppSettings["password"],
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
