using Swytch.Router.Structures;

namespace Swytch.Router.Structures;

/*This a Route object. It represents in a sense the route a request should take.
For this reason it contains all essential information to identify the request which is expected to use it.
Like the url path of the request that should come through, the Http method and ultimately the request handling method that will be called .
The Http  request handler is a Delegate which  takes in RequestContext as argument and returns Task. Any method that matches this signature can be registered
as handler for a route*/

internal class Route
{
    internal string[] UrlPath;
    internal Func<RequestContext, Task> RequestHandler;
    internal List<string> Methods = new List<string>();

    internal Route(Func<RequestContext, Task> handler, string[] path)
    {
        UrlPath = path;
        RequestHandler = handler;
    }
}