using RabbitMQ.Client;

namespace SRP.Solution
{
    public class EventConnectionFactory
    {
        public IConnection GetConnection()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "xpto",
                Password = "12345"
            };
            return connectionFactory.CreateConnection();
        }
    }
}
