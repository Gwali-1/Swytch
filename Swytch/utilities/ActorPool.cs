using System.Collections.Concurrent;
using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Routing;

namespace Swytch.utilities;

public static class ActorPool
{
    private static ActorSystem? _actorSystemPool;
    private  const   string ActorPoolName = "SWYTCHACTORSYSTEMPOOL";
    private static readonly ConcurrentDictionary<string, IActorRef> Actors = new();

    //register
    public static void Register<T>(int intances = 0) where T : ActorBase
    {
        if (_actorSystemPool is null)
        {
            throw new InvalidOperationException("Actor system pool not initialized");
        }


        var actorName = typeof(T).ToString();
        var routeeInstances = intances == 0 ? Environment.ProcessorCount : intances;
        if (Actors.ContainsKey(actorName))
        {
            throw new InvalidOperationException(
                $"Actor with name {actorName} has already been registered in the actor pool");
        }

        var resolver = DependencyResolver.For(_actorSystemPool);
        Props props = resolver.Props<T>()
            .WithRouter(new RoundRobinPool(routeeInstances, new DefaultResizer(routeeInstances, 10000)))
            .WithSupervisorStrategy(new OneForOneStrategy(maxNrOfRetries: 5, withinTimeRange: TimeSpan.FromSeconds(10),
                localOnlyDecider: _ => Directive.Restart));
        var actorRef = _actorSystemPool.ActorOf(props, actorName);
        Actors[actorName] = actorRef;
    }

    //tell
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
    public static void InitializeActorPool(IServiceProvider serviceProvider)
    {
        if (_actorSystemPool is null)
        {
            var actorSystemSetup = BootstrapSetup.Create().And(DependencyResolverSetup.Create(serviceProvider));
            _actorSystemPool = ActorSystem.Create(ActorPoolName, actorSystemSetup);
        }
    }

    //shutdown
    public static void ShutDown()
    {
        _actorSystemPool?.Terminate().Wait();
    }
}