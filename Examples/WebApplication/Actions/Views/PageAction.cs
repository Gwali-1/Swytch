using System.Net;
using Swytch.Structures;
using Swytch.utilities;

namespace WebApplication.Actions.Views;

public class PageAction
{
    public async Task HomePage(RequestContext context)
    {
        await Utilities.ServeFile(context, "LandingPage.html", HttpStatusCode.OK);
    }
}