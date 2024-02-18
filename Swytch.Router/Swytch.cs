using System.Collections.Specialized;
using System.Net;
using Swytch.Structures;

using Swytch.Utilies;

namespace Swytch.Router;



public class Swytch
{

    //registered routes
    private List<Route> _registeredRoutes = new List<Route>();
    private Queue<Func<HttpListenerContext, Task>> _registeredMiddlewares = new();
    private Func<HttpListenerContext, Task>? _swytchHandler;

    public Swytch() { }



    private Dictionary<string, string> GetQueryParams(HttpListenerContext c)
    {

        Dictionary<string, string> queryParams = new();
        NameValueCollection qParams = c.Request.QueryString;
        if (qParams.Count <= 0)
        {
            return queryParams;
        }

        foreach (var key in qParams.AllKeys)
        {
            queryParams[key] = qParams[key] ?? "";
        }
        return queryParams;
    }

    private (bool, RequestContext) MatchUrl(string url, Route r, HttpListenerContext c)
    {
        Dictionary<string, string> pathParams = new();
        RequestContext nc = new RequestContext(c);
        string[] urlSegements = url.Split("/");
        if (urlSegements.Length != r.urlPath.Length)
        {
            return (false, nc);
        }

        for (int i = 0; i < r.urlPath.Length; i++)
        {
            if (r.urlPath[i].StartsWith("{") && r.urlPath[i].EndsWith("}"))
            {
                if (!(string.IsNullOrWhiteSpace(urlSegements[i])))
                {
                    char[] trimChars = { '{', '}' };
                    string key = r.urlPath[i].Trim(trimChars);

                    pathParams[key] = urlSegements[i];
                    continue;

                }

                return (false, nc);
            }


            if (urlSegements[i] != r.urlPath[i])
            {
                return (false, nc);

            }
        }

        RequestContext newContext = new RequestContext(c);
        Dictionary<string, string> queryParams = GetQueryParams(c);

        newContext.PathParams = pathParams;
        newContext.QueryParams = queryParams;


        return (true, newContext);


    }




    public void NotFound(HttpListenerContext requestContext)
    {
        String responseBody = "NOT FOUND (404)";
        ResponseWriter.WriteStringToStream(requestContext, responseBody, HttpStatusCode.NotFound);
    }


    public void MethodNotAllowed(HttpListenerContext requestContext)
    {
        String responseBody = "METHOD NOT ALLOWED (405)";
        ResponseWriter.WriteStringToStream(requestContext, responseBody, HttpStatusCode.MethodNotAllowed);
    }


    //adds middleware in the order in which they were registered
    public void UseAsMiddleWare(Func<HttpListenerContext, Task> middleware)
    {
        _registeredMiddlewares.Enqueue(middleware);
    }


    //register a url, http methods and a handler method
    public void MapRoute(string methods, string url, Func<HttpListenerContext, Task> requestHandler)
    {
        String[] urlPath = url.Split("/");
        String[] meths = methods.Split(",");

        Route newRoute = new Route(requestHandler, urlPath);
        foreach (String method in meths)
        {
            newRoute.methods.Add(method);
        }
        _registeredRoutes.Add(newRoute);

    }



    private async Task SwytchHandler(HttpListenerContext r)
    {
        //impleentation("router matching logic here")
        await Task.Delay(0);
    }




    public Func<HttpListenerContext, Task> GetSwytchHandler()
    {
        if (_swytchHandler == null)
        {
            Swytch swytchInstance = new Swytch();
            Func<HttpListenerContext, Task> handler = swytchInstance.SwytchHandler;

            foreach (var f in _registeredMiddlewares)
            {
                handler = f + handler;
            }
            _swytchHandler = handler;
        }

        return _swytchHandler;
    }
}
