using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swytch.Extensions;
using Swytch.Structures;
using Swytch.utilities;
using WebApplication.Actors;

namespace WebApplication.Actions.Views;

public class PageAction
{
    private readonly ILogger<PageAction> _logger;
    public PageAction(IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetRequiredService<ILogger<PageAction>>();

    }
    public async Task HomePage(RequestContext context)
    {
        _logger.LogInformation("Home page request");
        ActorPool.Tell<TalkingActor,string>("Home");
        await context.ServeFile("LandingPage.html", HttpStatusCode.OK);
    }
}