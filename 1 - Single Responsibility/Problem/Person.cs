using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text.RegularExpressions;

namespace SRP.Problem
{
    /// This class represents a person
    public class Person
    {
        public int Id { get; set; }
        public string ITIN { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }

        /// Verify if this data is valid
        public bool Validate()
        {
            return string.IsNullOrWhiteSpace(Name) && BirthDate == DateTime.MinValue && !ValidateITIN(ITIN);
        }

        /// Validate Individual Taxpayer Identification Number (ITIN)
        public static bool ValidateITIN(string document)
        {
            return new Regex(@"^(9\d{2})([ \-]?)([7]\d|8[0-8])([ \-]?)(\d{4})$").IsMatch(document);
        }

        /// Save person
        public void Save()
        {
            const string queueName = "Person";

            // Get rabbitMQ connection 
            IConnection connection = GetConnection();
            // Create Queue
            CreateQueue(queueName, connection);
            // Publish
            Publish(JsonConvert.SerializeObject(this), queueName, connection);
        }

        /// Get the RabbitMQ connection
        public IConnection GetConnection()
        {
            return new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "xpto",
                Password = "12345"

            }.CreateConnection();
        }

        /// Create the de queue
        public QueueDeclareOk CreateQueue(string queueName, IConnection connection)
        {
            QueueDeclareOk queue;
            using (var channel = connection.CreateModel())
            {
                queue = channel.QueueDeclare(queueName, false, false, false, null);
            }
            return queue;
        }

        /// Publish the data
        public bool Publish(string message, string queueName, IConnection connection)
        {
            using (var channel = connection.CreateModel())
            {
                channel.BasicPublish(string.Empty, queueName, null, Encoding.ASCII.GetBytes(message));
            }
            return true;
        }
    }
}
