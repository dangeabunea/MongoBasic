using System;

namespace MongoBasic.Core
{
    public sealed class MongoBasicException
    {
        public sealed class ConfigurationException : Exception
        {
            public ConfigurationException()
            {

            }

            public ConfigurationException(String message)
                : base(message)
            {

            }

            public ConfigurationException(String message, Exception inner)
                : base(message, inner)
            {

            }
        }

        public sealed class CollectionNotFoundException : Exception
        {
            public CollectionNotFoundException()
            {

            }

            public CollectionNotFoundException(String message)
                : base(message)
            {

            }

            public CollectionNotFoundException(String message, Exception inner)
                : base(message, inner)
            {

            }
        }

        public sealed class InvalidEntityException : Exception
        {
            public InvalidEntityException()
            {

            }

            public InvalidEntityException(String message)
                : base(message)
            {

            }

            public InvalidEntityException(String message, Exception inner)
                : base(message, inner)
            {

            }
        }
    }
}
