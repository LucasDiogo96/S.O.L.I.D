using MongoDB.Driver;

namespace DIP.Problem
{
    public class MongoDBRepository
    {
        public void Save(Person person)
        {
            const string connectionString = "mongodb://localhost:27017";

            var client = new MongoClient(connectionString);

            var database = client.GetDatabase("example");

            var collection = database.GetCollection<Person>("Person");

            collection.InsertOne(new Person { Name = person.Name });
        }
    }
}
