# MongoBasic
MongoBasic is a light facade on top of the Mongo C# Driver. It was mainly built to eliminate some of the verbosities of working with the original Mongo C# driver like the need to specify collection names as string, the dificulty of using an auto-incremented id value or the slightly complex way of deleting entities. It alos provides a more clear approach to mapping entities, similar to the one used in other popular ORM tools.

# Compatibility

MongoBasic depends on the official Mongo C# Driver, version 1.10.0. This means that the async features are not present, but it ensures a great cross paltform compatibility with both the Mongo Database and with the .NET and Mono runtimes. 
* Supports MongoDB 2.4, 2.6, 3.x
* Supports .NET Framework 4.x
* Supports Mono 3.x, 4.x

For more details regarding this, you can check out the official driver page : http://docs.mongodb.org/ecosystem/drivers/csharp/

# Installation

https://www.nuget.org/packages/MongoBasic/

# Getting started
