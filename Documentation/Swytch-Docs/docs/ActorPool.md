### Swytch Actor Pool

Swytch provides built-in support for concurrent and background task execution using the actor model.
This is made possible through a preconfigured actor pool built on top of the powerful Akka.NET framework.
This is achieved by exposing APIs that allow you to send tasks to an actor pool or directly to an individual actor.
Under the hood, Swytch leverages **Akka.NET**, a powerful actor system implementation, and exposes a simplified API to
use
it seamlessly within your application.

---

### Understanding the Actor Model

The **Actor Model** is a concurrency model that treats "actors" as the fundamental units of computation.
Unlike traditional concurrency approaches in C#—like using `Threads`, `Tasks`, or the `ThreadPool`—where shared memory
and
synchronization (e.g., locks, semaphores) are common, the actor model promotes a **message-passing architecture**.

Here’s how it differs and works:

- **No Shared State**: Each actor encapsulates its own state. This removes the need for locks or mutexes to protect
  shared data.
- **Message Passing**: Actors communicate exclusively through asynchronous messages. One actor sends a message to
  another, which is then processed in isolation.
- **Isolation and Fault-Tolerance**: Since actors don't share memory, failures in one actor do not affect others. They
  can also supervise and restart each other in case of failures.
- **Sequential Execution per Actor**: Each actor processes one message at a time, avoiding race conditions by design.

This model makes concurrent and parallel execution more robust, scalable, and easier to reason about—especially in
complex systems.
This are some useful links to read more about the actor model

[Actor-Model System with Akka.NET](https://www.zartis.com/actor-model-system-with-akka-net/)

[When and How to Use the Actor Model An Introduction to Akka NET Actors](https://youtu.be/0KnIMDoJpZs)

---

### Swytch Actor Pool

Swytch makes it easy to use the actor model through a static utility class called `ActorPool`.
This class provides methods for initializing the pool, registering your actor types, and sending messages to them.

Swytch internally leverages Akka.NET to implement its actor system. It wraps some of the Akka.NET complexity and
exposes an easy-to-use utility class called ActorPool that allows you to initialize and interact with the actor
infrastructure.

Let's explore how to use it:

Before using the actor pool, you must initialize it with your application's `IServiceProvider`:

### InitializeActorPool(serviceProvider)

The `InitializeActorPool` method is used to initialize the actor pool for your Swytch application.
This method requires the `IServiceProvider` parameter to set up the actor pool in such a way that your actors can
leverage dependency injection (DI).
By passing the service provider to this method, you enable your actors to automatically resolve and use any services
registered in the DI container. This simplifies the implementation of actors because you don't have to manually manage
dependencies within them. Instead, you register all the services an actor might need in the service provider and pass
that
provider when initializing the actor pool. The pool ensures that each actor is properly constructed with its
dependencies
injected when executed.

This method is essential and **must** be called first. If you attempt to use the actor pool or any related methods
before
calling this method, an `InvalidOperationException` will be thrown with the message:

#### Example

```csharp
 // Create a service collection and register the services
   var serviceCollection = new ServiceCollection();
   serviceCollection.AddSingleton<MyService>();
   serviceCollection.AddActors(); // Assuming you have a method to register actor services

 // Build the service provider
    var serviceProvider = serviceCollection.BuildServiceProvider();

 // Initialize the actor pool with the service provider
 ActorPool.InitializeActorPool(serviceProvider);

```


### Register<T>(int instances = 0)

After initializing your actor pool using the `InitializeActorPool` method, you can register your custom actor implementations into the pool. The `Register<T>` method allows you to register an actor that will be used for future executions.

This method takes a class of type `T` that must inherit from `ActorBase`, which marks it as a valid actor in Swytch. By doing this, Swytch adds your actor to its internal actor pool, where it is equipped with routers to manage and execute tasks concurrently.

Swytch automatically adjusts the number of actor instances in the pool based on demand, with an upper limit of one million instances. This ensures that the system dynamically scales by creating more actor instances when necessary to improve throughput and execution speed, while also preventing resource wastage by maintaining a manageable number of instances when demand is low.

You can pass an optional `instances` parameter to specify the minimum number of actor instances you want to be available in the pool at all times. This allows you to ensure that a certain number of actor instances are ready to handle tasks, even during low-demand periods.

**Important**: You cannot register the same actor type multiple times. If you attempt to do so, an `InvalidOperationException` will be thrown.

Once an actor is registered, you can send messages to it using the `Tell` method to trigger actions in the actor.

#### Example

Let's look at a simple example of an actor that receives a message and logs it:

```csharp
public class LoggingActor : ReceiveActor
{
    private readonly ILogger<LoggingActor> _logger;

    public LoggingActor(ILogger<LoggingActor> logger)
    {
        _logger = logger;
        
        // Define how the actor will handle messages of type string
        Receive<string>(message =>
        {
            _logger.LogInformation($"Logging Actor recieved message = {message}");
        });
    }
}

```

The LoggingActor class inherits from Swytch's ReceiveActor base class, which is a base class provided by Akka.NET and Swytch for handling 
messages enabling it to handle incoming messages. 
It uses dependency injection to receive an ILogger<LoggingActor>, which allows it to log messages. In the actor's constructor,
the logger is injected, and the actor is set up to handle messages of type string. When a message of this type is received,
the actor logs the message using the injected logger. This shows how actors in Swytch can use dependency injection for
external services and perform specific actions, such as logging, upon receiving messages.


**Register the actor**

```csharp
ActorPool.Register<LogActor>(instances: 2); // Register LogActor with a minimum of 2 instances

```
This registeres and ensures that there will always be at least 2 instances of the LogActor available to process messages. 
Swytch will handle scaling the number of actor instances based on the system's demand.


### ActorPool.Tell

Once registered, you can send a message to the actor using the Tell method:
The Tell method is used to send a message to a registered actor in the actor pool. This method takes two parameters:

T specifies the type of the actor that will receive the message.

TM is the type of the message being sent to the actor.

When a message is sent using this method, Swytch identifies the actor in the pool by the actor type T, and it sends 
the specified message of type TM to it. The actor must have a handler that can process messages of type TM. 
If an actor is not registered in the pool or if the actor does not have a handler for the provided message type,
nothing happens, and in the case of an unregistered actor, an InvalidOperationException will be thrown.

This method allows for flexible message passing to actors, ensuring that the actor processes messages of
the correct type, while also ensuring that only valid actors in the pool handle the incoming messages

#### Example

```csharp
ActorPool.Tell<LogActor, string>("Hello, Actor!");

```

In this example, the Tell method sends the message "Hello, Actor!" to one of the available LogActor instances,
which will then log the message to the console.

By registering your actors and sending messages, you can easily build concurrent, scalable applications using the actor model.

### Complete Example

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using Swytch.ActorSystem;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

// Define the actor class that inherits from ReceiveActor
public class LoggingActor : ReceiveActor
{
    private readonly ILogger<LoggingActor> _logger;

    // Constructor that takes in the injected logger
    public LoggingActor(ILogger<LoggingActor> logger)
    {
        _logger = logger;

        // Define how the actor should handle messages of type string
        Receive<string>(message =>
        {
            _logger.LogInformation($"Logging Actor received message: {message}");
        });
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        // Step 1: Set up the service container and register necessary services
        var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole()) // Register logging service
            .BuildServiceProvider();

        // Step 2: Initialize the actor pool using the service provider
        ActorPool.InitializeActorPool(serviceProvider);

        // Step 3: Register the LoggingActor with the actor pool
        ActorPool.Register<LoggingActor>(2); // Registering 2 instances of LoggingActor

        // Step 4: Send a message to the registered LoggingActor
        // This will invoke the Receive method inside LoggingActor and log the message
        ActorPool.Tell<LoggingActor, string>("Hello, Actor!"); // This sends the message to the LoggingActor

        // Ensure that the actor pool is properly cleaned up if necessary (e.g., if using a real application setup)
        await Task.Delay(1000); // Wait for the log to be written before application exits
    }
}

```