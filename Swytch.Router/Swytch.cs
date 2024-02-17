using System.Net;
using Swytch.Structures;

using Swytch.Utilies;

namespace Swytch.Router;



public class Swytch
{

    //registered routes

    private List<Route> _registeredRoutes = new List<Route>();
    private List<Func<HttpListenerContext, Task>> _registeredMiddlewares = new();


    public void NotFound(HttpListenerContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        String responseBody = "NOT FOUND (404)";
        Utilities.WriteStringToStream(context, responseBody);
    }


    public void MethodNotAllowed(HttpListenerContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
        String responseBody = "METHOD NOT ALLOWED (405)";
        Utilities.WriteStringToStream(context, responseBody);
    }
}
