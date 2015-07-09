namespace MongoBasic.Core
{
    public interface IMongoSessionFactory
    {
        IMongoSession OpenSession<T>() where T : AbstractMongoSession;
    }
}