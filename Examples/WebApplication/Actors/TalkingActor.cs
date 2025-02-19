using Akka.Actor;
using Microsoft.Extensions.Logging;

namespace WebApplication.Actors;

public class TalkingActor : ReceiveActor
{
    private readonly ILogger<TalkingActor> _logger;

    public TalkingActor(ILogger<TalkingActor> logger)
    {
        _logger = logger;
        Receive<string>(message =>
        {
            _logger.LogInformation($"Talking actor {Context.Self.ToString()} says {message}");
        });
    }
}