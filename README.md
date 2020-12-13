# S.O.L.I.D

There are five principles to follow when we write code with the object oriented programming to make it more readble and maintainable if we doesn't want that our program becomes a clock bomb in our hands. This principles together compose the acronym SOLID that means respectively.

 [**S**ingle responsiblity principle.](#srp)

[**O**pen-closed principle.](#ocp)

**L**iskov substitution principle.

**I**nterface segregation principle.

**D**ependency Inversion Principle.

Witch one of these principles has a big importance and together it will make the architecture of our code as the acronym says , a solid architecture, something that isn't unstable.

So, below we will aproach each one of these principles briefly to turn the undestanding less complex.

<div id='srp'/>  

## Single responsiblity principle.

This principle says:

`A class should have one and only one reason to change`

It means that a class must only be responsible for one context, that is, one thing and what isn't of its context will be out of the class.

Below there is an example where we have more thant one responsability for our class.

### The problem

```csharp
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
```
If we try to count it we will realize that there are at least 5 responsibilities that shouldn't be there. So, the person class not must know how to persist itself or how to validate an individual taxpayer identification or how to create and how to use the rabbitMQ  as well.

### The solution

So we need to segregrate each responsabilitie in different classes so if something change we know that we just need to change in that class and it will not affect the others components and it will make out code more testable and readble. :smiley:


The responsabilite of person is provide a valid data.
```csharp
    public class Person
    {
        public int Id { get; set; }
        public string ITIN { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && 
                   BirthDate != DateTime.MinValue && 
                  !IdentificationService.ValidateITIN(ITIN);
        }
    }
```
The indentification service has only the responsabilitie of tell us if the ITIN is valid.

```csharp
  public static class IdentificationService
    {
        public static bool ValidateITIN(string document)
        {
            return new Regex(@"^(9\d{2})([ \-]?)([7]\d|8[0-8])([ \-]?)(\d{4})$").IsMatch(document);
        }
    }
```
In the EventConnectionFactory just to provide us the connection.

```csharp
   public class EventConnectionFactory
    {
      public IConnection GetConnection()
        {
            return new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "xpto",
                Password = "12345"

            }.CreateConnection();
        }
    }
```

The EventBusService will just only has the behavior of RabbitMQ.

```csharp
 public class EventBusService
    {
        public QueueDeclareOk CreateQueue(string queueName, IConnection connection)
        {
            QueueDeclareOk queue;
            using (var channel = connection.CreateModel())
            {
                queue = channel.QueueDeclare(queueName, false, false, false, null);
            }
            return queue;
        }

        public bool Publish(string message, string queueName, IConnection connection)
        {
            using (var channel = connection.CreateModel())
            {
                channel.BasicPublish(string.Empty, queueName, null, Encoding.ASCII.GetBytes(message));
            }
            return true;
        }
    }
```

And finnaly we should use a business class to create our business logic to persist the person data.

```csharp
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
```

<div id='ocp'/>  

## Open-closed Principle.

This principle says:

`Objects or entities should be open for extension, but closed for modification.`

A brief summary of it , it means that a class should be easily extendable without modifying the class itself.
