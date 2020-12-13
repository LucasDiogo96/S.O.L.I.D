using Newtonsoft.Json;
using RabbitMQ.Client;

namespace SRP.Solution
{
    public class PersonBusiness
    {
        const string queueName = "person";

        public bool Save(Person person)
        {
            if (person.IsValid())
            {
                EventConnectionFactory connectionFactory = new EventConnectionFactory();

                // Get rabbitMQ connection 
                IConnection connection = connectionFactory.GetConnection();

                EventBusService busService = new EventBusService();

                // Create Queue
                busService.CreateQueue(queueName, connection);

                // Publish
                busService.Publish(JsonConvert.SerializeObject(person), queueName, connection);

                return true;
            }

            return false;
        }
    }
}
