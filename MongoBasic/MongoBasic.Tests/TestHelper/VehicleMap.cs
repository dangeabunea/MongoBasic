using MongoBasic.Core;
using MongoDB.Bson.Serialization;

namespace MongoBasic.Tests.TestHelper
{
    public class VehicleMap:MongoClassMap<Vehicle>
    {
        public VehicleMap(IIdGenerator idGenerator):base(idGenerator)
        {
            
        }

        protected override void MapEntity(BsonClassMap<Vehicle> cm)
        {
            cm.MapIdField(x => x.Id).SetIdGenerator(this.IdGenerator);
            cm.UnmapProperty(x => x.DateOfProduction);
            cm.AutoMap();
        }
    }
}
