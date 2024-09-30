using System.Net;
using Microsoft.Extensions.Logging;
using Swytch.Structures;
using Swytch.utilities;

namespace WebApplication.Actions.Views;

public class PageAction
{
    private readonly ILogger<PageAction> _logger;
    public PageAction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<PageAction>();

    }
    public async Task HomePage(RequestContext context)
    {
        _logger.LogInformation("Home page request");
        await Utilities.ServeFile(context, "LandingPage.html", HttpStatusCode.OK);
    }
}