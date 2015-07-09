namespace MongoBasic.Core
{
    public sealed class MongoSessionFactoryConfig
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public MongoCredentialType CredentialType { get; set; }

        public MongoSessionFactoryConfig()
        {
            Server = "localhost";
            Port = 25017;
            CredentialType = MongoCredentialType.MONGODB_CR;
        }
    }
}
