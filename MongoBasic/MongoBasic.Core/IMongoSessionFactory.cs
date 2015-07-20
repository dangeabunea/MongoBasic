namespace MongoBasic.Core
{
    public interface IMongoSessionFactory
    {
        IMongoSession OpenSession();

    }
}