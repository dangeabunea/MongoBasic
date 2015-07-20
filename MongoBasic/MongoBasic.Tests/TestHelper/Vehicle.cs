using System;
using MongoDB.Bson;

namespace MongoBasic.Tests.TestHelper
{
    public class Vehicle
    {
        public long Id { get; set; }
        public string Model { get; set; }
        public string Make { get; set; }
        public DateTime DateOfProduction { get; set; }
    }
}
