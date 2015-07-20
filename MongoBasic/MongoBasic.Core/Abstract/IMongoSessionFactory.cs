namespace MongoBasic.Core.Abstract
{
    public interface IMongoSessionFactory
    {
        IMongoSession OpenSession();

    }
}