using System.Collections.Specialized;
using System.Net;
using Swytch.Structures;
using Swytch.Utilies;

namespace Swytch.Router;



public class Swytch
{

    //registered routes
    private List<Route> _registeredRoutes = new List<Route>();
    private Queue<Func<RequestContext, Task>> _registeredMiddlewares = new();
    private Func<RequestContext, Task>? _swytchHandler;

    public Swytch() { }



    public async Task NotFound(RequestContext requestContext){
        String responseBody = "NOT FOUND (404)";
        await Utilities.WriteStringToStream(requestContext, responseBody, HttpStatusCode.NotFound);
    }


    public async Task MethodNotAllowed(RequestContext requestContext)
    {
        String responseBody = "METHOD NOT ALLOWED (405)";
        await Utilities.WriteStringToStream(requestContext, responseBody, HttpStatusCode.MethodNotAllowed);
    }


    //adds middleware in the order in which they were registered
    public void UseAsMiddleWare(Func<RequestContext, Task> middleware)
    {
        _registeredMiddlewares.Enqueue(middleware);
    }


    //register a url, http methods and a handler method
    public void MapRoute(string methods, string url, Func<RequestContext, Task> requestHandler)
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


    public async Task Listen(string addr)
    {
        try
        {
            HttpListener server = new HttpListener();
            server.Prefixes.Add(addr);
            server.Start();
            Console.WriteLine($"Server is live at {addr}");

            while (true)
            {
                HttpListenerContext ctx = await server.GetContextAsync();
                Func<RequestContext, Task> hndler = this.GetSwytchHandler();
                await hndler(new RequestContext(ctx));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Server Stopped Unexpectedly ");
            Console.WriteLine(e.Message);
            throw;

        }

    }


    private Dictionary<string, string> GetQueryParams(RequestContext c)
    {

        Dictionary<string, string> queryParams = new();
        NameValueCollection qParams = c.Request.QueryString;
        if (qParams.Count <= 0)
        {
            return queryParams;
        }

        foreach (var key in qParams.AllKeys)
        {
            if (key is not null)
            {

                queryParams[key] = qParams[key] ?? "";
            }
        }
        return queryParams;
    }


    private (bool, RequestContext) MatchUrl(string url, Route r, RequestContext c)
    {
        Dictionary<string, string> pathParams = new();
        string[] urlSegements = url.Split("/");
        if (urlSegements.Length != r.urlPath.Length)
        {
            return (false, c);
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

                return (false,c);
            }


            if (urlSegements[i] != r.urlPath[i])
            {
                return (false, c);

            }
        }

        Dictionary<string, string> queryParams = GetQueryParams(c);

        c.PathParams = pathParams;
        c.QueryParams = queryParams;


        return (true, c);


    }



    private async Task SwytchHandler(RequestContext c)
    {

        string url = c.Request.Url.AbsolutePath;

        foreach (Route r in _registeredRoutes)
        {
            var (matched, context) = MatchUrl(url, r, c);
            if (matched)
            {
                if (r.methods.Contains(c.Request.HttpMethod))
                {
                    await  r.requestHandler(context);
                    return;
                }
                //return with methof not allowed
                await MethodNotAllowed(c);
                return;
            }

        }

        await NotFound(c);
    }



    private Func<RequestContext, Task> GetSwytchHandler()
    {
        if (_swytchHandler == null)
        {
            Func<RequestContext, Task> handler = this.SwytchHandler;

            foreach (var f in _registeredMiddlewares)
            {
                handler = f + handler;
            }
            _swytchHandler = handler;
        }

        return _swytchHandler;
    }
}
