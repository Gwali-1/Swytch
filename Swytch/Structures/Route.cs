namespace Swytch.Structures;

/*This a Route class. It represents in a sense the route a request should take.
For this reason it contains all essential information to identify the request which is expected to use it.
Like the url path of the request that should come through, the HTTP methods and ultimately the request handling method that will be called.
The HTTP  request handler is a Delegate which  takes in RequestContext as argument and returns Task. Any method that matches this signature can be registered
as handler for a route*/

internal class Route
{
    internal string[] UrlPath;
    internal Func<RequestContext, Task> RequestHandler;
    internal List<RequestMethod>  Methods = new List<RequestMethod>();

    internal Route(Func<RequestContext, Task> handler, string[] path)
    {
        UrlPath = path;
        RequestHandler = handler;
    }
}