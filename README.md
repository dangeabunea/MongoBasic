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

# Show me some code

````c#
//create a simple POCO that will be saved in the database
public class Person
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}
````

````c#
//create a database mapping for the Person class
public class PersonMap: MongoClassMap<Person>
{
    protected override void MapEntity(BsonClassMap<Person> cm)
    {
      //mapping is done using the official mongo csharp driver
      cm.AutoMap();
    }
}
````

````c#
//create concrete implementation of mongo session factory
public class MyMongoSessionFactory:AbstractMongoSessionFactory
{
    public MyMongoSessionFactory(MongoSessionFactoryConfig configuration) : base(configuration){}

    protected override void AfterSessionFactoryInitialized(MongoDatabase mongoDatabase)
    {
        //register your defined mappings
        RegisterClassMap(new PersonMap());
    }
}
````

````c#
//the main class
class Program
  {
    static void Main(string[] args)
    {
      //create mongo config
      MongoSessionFactoryConfig mongoConfig = new MongoSessionFactoryConfig("mongodb://localhost:27016/personsdb?safe=true")
      {
        //define the collections
        Collections = new Dictionary<Type, string>()
        {
          //objects of type Person will be saved in the Persons collections
          {typeof (Person), "Persons"}
        }
      };

      //create an instance of your concrete mongo session factory
      var mongoSessionFactory = new MyMongoSessionFactory(mongoConfig);

      //open a mongo session
      IMongoSession mongoSession = mongoSessionFactory.OpenSession();

      //save a person
      Person person = new Person
      {
        Name = "Scott",
        Age = 29
      };
      mongoSession.Save(person);

      //retrieve objects in the person collection
      var savedPersons = mongoSession.Query<Person>();

      //print
      foreach (var savedPerson in savedPersons)
      {
        Console.WriteLine(savedPerson.Name);
      }
      Console.ReadLine();
  }
}
````

# Show me more code

If you want to explore all the capabilities of this library, you can check out the wiki section.
