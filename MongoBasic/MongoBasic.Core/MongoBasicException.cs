using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoBasic.Core
{
    public sealed class MongoBasicException
    {
        public sealed class ConfigurationException : Exception
        {
            public ConfigurationException()
            {
               
            }

            public ConfigurationException(String message):base(message)
            {
                
            }

            public ConfigurationException(String message, Exception inner):base(message,inner)
            {
                
            }
        }
    }
}
