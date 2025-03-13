using System.Collections.Concurrent;
using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Routing;

namespace Swytch.utilities;


/// <summary>
/// The Actor pool is a public static class the exposes various public methods that allow you to initialize an actor pool, register actors and activate them by sending them
/// messages.
/// </summary>
public static class ActorPool
{
    private static ActorSystem? _actorSystemPool;
    private const string ActorPoolName = "SWYTCHACTORSYSTEMPOOL";
    private static readonly ConcurrentDictionary<string, IActorRef> Actors = new();

    //register
    
    /// <summary>
    /// Registers a valid actor in the Swytch actor pool
    /// </summary>
    /// <param name="intances">minimum number of instances of your actor. Defaults to the number of processors in the run environment. The maximum amount of instances that will be
    /// provisioned based on requirement in the pool is capped at a  million(1000000) </param>
    /// <typeparam name="T">The type of your actor</typeparam>
    /// <exception cref="InvalidOperationException">If actor pool is not initialized or multiple registration of the same actor</exception>
    public static void Register<T>(int intances = 0) where T : ActorBase
    {
        if (_actorSystemPool is null)
        {
            throw new InvalidOperationException("Actor system pool not initialized");
        }

        var actorName = typeof(T).ToString();
        var lowerBoundInstances = intances == 0 ? Environment.ProcessorCount : intances;
        if (Actors.ContainsKey(actorName))
        {
            throw new InvalidOperationException(
                $"Actor with name {actorName} has already been registered in the actor pool");
        }

        var resolver = DependencyResolver.For(_actorSystemPool);
        Props props = resolver.Props<T>()
            .WithRouter(new RoundRobinPool(lowerBoundInstances, new DefaultResizer(lowerBoundInstances, 100000)))
            .WithSupervisorStrategy(new OneForOneStrategy(maxNrOfRetries: 5, withinTimeRange: TimeSpan.FromSeconds(10),
                localOnlyDecider: _ => Directive.Restart));
        var actorRef = _actorSystemPool.ActorOf(props, actorName);
        Actors[actorName] = actorRef;
    }

    
    /// <summary>
    /// Sends a message to an previously registered actor in the pool.
    /// </summary>
    /// <param name="message">The message to send to the actor</param>
    /// <typeparam name="T">The type of your actor</typeparam>
    /// <typeparam name="TM">The type of your message</typeparam>
    /// <exception cref="InvalidOperationException">If actor does not exist in the pool ie not previously registered</exception>
    public static void Tell<T, TM>(TM message)
    {
        var actorName = typeof(T).ToString();
        if (Actors.TryGetValue(actorName, out var actorRef))
        {
            actorRef.Tell(message);
        }
        else
        {
            throw new InvalidOperationException($"No Actor with name {actorName} registered in pool");
        }
    }

    //create an actor system
    /// <summary>
    /// Initializes and configures  the actor system (actor pool). Call this method first before registering actors or you will get InvalidOperationException 
    /// </summary>
    /// <param name="serviceProvider">The service provider from the service container of your registered services </param>
    public static void InitializeActorPool(IServiceProvider serviceProvider)
    {
        if (_actorSystemPool is null)
        {
            var actorSystemSetup = BootstrapSetup.Create().And(DependencyResolverSetup.Create(serviceProvider));
            _actorSystemPool = ActorSystem.Create(ActorPoolName, actorSystemSetup);
        }
    }

    /// <summary>
    /// Shut down the actor pool
    /// </summary>
    public static void ShutDown()
    {
        _actorSystemPool?.Terminate().Wait();
    }
}