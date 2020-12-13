# S.O.L.I.D

There are five principles to follow when we write code with the object oriented programming to make it more readble and maintainable if we doesn't want that our program becomes a clock bomb in our hands. This principles together compose the acronym SOLID that means respectively.

 [**S**ingle responsiblity principle.](#srp)

[**O**pen-closed principle.](#ocp)

[**L**iskov substitution principle.](#lsp)

[**I**nterface segregation principle.](#isp)

[**D**ependency Inversion Principle.](#dip)


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

### The problem

Let's imagine we have a class called Check,and the context of this class is mark a presence.

```csharp
      public class Check
    {
        public int Id { get; set; }
        public CheckTypeEnum Type { get; set; }
        public string Justification { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime ExitDate { get; set; }
    }
```
And here we have the type of this check if basically if it's a checkin or a checkout

```csharp
  public enum CheckTypeEnum
    {
        [Description("Check-IN")]
        IN,    
        [Description("Check-Out")]
        OUT
    }
```
And when we try to do something with this data most of programmers be based in the enum type, and if tomorrow there is a new type of check? Probably you are thinking... I just need to put a new enum type e write my code based in this kind of check.


```csharp
   public void Save(Check check)
        {
            if (check.Type == CheckTypeEnum.IN)
            {         
                //DO SOMETHING
            }
            else if (check.Type == CheckTypeEnum.OUT)
            {
                //DO SOMETHING ELSE
            }
        }
```

It's a bad practice of this because you will modify the code that was already working and it can be dangerous and also you won't make a good unity test.

How to solve it?

### The solution

In this case we will create a [abstract class](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/abstract) to isolate our business logics using the a override for each kind of check.


```csharp
    public abstract class CheckService
    {
        public abstract void CreateCheck(Check check);
    }
```

And here  is icing on the cake. :cake:

As i said above, we can isolate the business logics for each type of check using inheritance and we can write a new functionality without changing the existent code and it will prevent situations like we are changing something in the class and needs to adapt out depending classes.


```csharp
     public class Checkin : CheckService
     {
         public override void CreateCheck(Check check)
         {
             //our business rules for a checkout
         }
     }
     
    public class Checkout : CheckService
    {
        public override void CreateCheck(Check check)
        {
           //our business rules for a checkout
        }
    } 
    
```

<div id='lsp'/>  

## Liskov substitution principle.

This principle says:

`Let q(x) be a property provable about objects of x of type T. Then q(y) should be provable for objects y of type S where S is a subtype of T.`

If you read it you can be confused but translating it for the development world it becomes.

`Derived classes must be substitutable for their base classes.`

It's better now rigth ? :smile:

### The problem.

The following code approaches it

We have a class **Fruit**, this class has a method that can be overrided

```csharp
    public abstract class Fruit
    {
        public abstract string GetColor();
    }
```  

Here we have a class Apple that extends fruit 

```csharp
    public class Apple : Fruit
    {
        public override string GetColor() => "Red";
    }
```

And here is a class that extends Apple. Pay attention in this situation. If there is a class fruit , and a Apple extends fruit and Orange extends of Apple we can presume that Orange is an Fruit too rigth?

```csharp
    public class Orange : Apple
    {
        public override string GetColor() => "Orange";
    }
```  

Now we will run the code below 


```csharp
    public class Orange : Apple
    {
            Fruit fruit = new Apple();
            Console.WriteLine("An apple is " + fruit.GetColor());
         
            fruit = new Orange();
            Console.WriteLine("An orange is " + fruit.GetColor());
        
    }
``` 

We will get the following result


`An apple is Red`

`An orange is Red`


It happens because the orange is using  the apple override and not itself override because we using the Apple inheritance. We must take care when we use inheritance because if the code its not respecting this principle the behavior of the funcionality can be opposite of what we want.

Let's imagine that we are coding an application to throw a missile, in this case, we will throw it to the wrong place and it can be expensive and harmful.


### The solution.

In this case the Orange class just needs to implements the fruit inheritance because we are using a abstract class so  the Orange class will has the itself implementation

```csharp
    public class Orange : Fruit
    {
        public override string GetColor() => "Orange";
    }
``` 

`An apple is Red`

`An orange is Orange`
