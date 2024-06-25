using System.Collections.Specialized;
using System.Net;
using Swytch.Router.Structures;
using Swytch.Router.utilities;

namespace Swytch.Router;

public class Swytch
{
    //registered routes
    private readonly List<Route> _registeredRoutes = new List<Route>();
    private readonly Queue<Func<RequestContext, Task>> _registeredMiddlewares = new();
    private Func<RequestContext, Task>? _swytchRouter;
    private readonly Dictionary<string, byte[]> _staticFiles = new();


    //adds middleware in the order in which they were registered
    public void AddMiddleWare(Func<RequestContext, Task> middleware)
    {
        _registeredMiddlewares.Enqueue(middleware);
    }


    //register a url, http methods and a handler method
    public void AddAction(string methods, string path, Func<RequestContext, Task> requestHandler)
    {
        string[] urlPath = path.Split("/");
        string[] meths = methods.Split(",");

        Route newRoute = new Route(requestHandler, urlPath);

        foreach (String method in meths)
        {
            newRoute.Methods.Add(method);
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
                Func<RequestContext, Task> hndler = GetSwytchRouter();
                try
                {
                    _ = Task.Run(() => hndler(new RequestContext(ctx)));
                }
                catch (Exception e)
                {
                    await InternalServerError(new RequestContext(ctx));
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Make sure address provided matches scheme http:// or https:// or ends with /");
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine("Server Stopped Unexpectedly");
            Console.WriteLine(e.Message);
            throw;
        }
    }


    private static Dictionary<string, string> GetQueryParams(RequestContext c)
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


    private static async Task NotFound(RequestContext requestContext)
    {
        string responseBody = "NOT FOUND (404)";
        await Utilities.WriteStringToStream(requestContext, responseBody, HttpStatusCode.NotFound);
    }

    private static async Task InternalServerError(RequestContext requestContext)
    {
        string responseBody = "INTERNAL SERVER ERROR (500)";
        await Utilities.WriteStringToStream(requestContext, responseBody, HttpStatusCode.NotFound);
    }

    private static async Task MethodNotAllowed(RequestContext requestContext)
    {
        string responseBody = "METHOD NOT ALLOWED (405)";
        await Utilities.WriteStringToStream(requestContext, responseBody, HttpStatusCode.MethodNotAllowed);
    }


    private static (bool, RequestContext) MatchUrl(string url, Route r, RequestContext c)
    {
        Dictionary<string, string> pathParams = new();
        string[] urlSegements = url.Split("/");
        if (urlSegements.Length != r.UrlPath.Length)
        {
            return (false, c);
        }

        for (int i = 0; i < r.UrlPath.Length; i++)
        {
            if (r.UrlPath[i].StartsWith("{") && r.UrlPath[i].EndsWith("}"))
            {
                if (!(string.IsNullOrWhiteSpace(urlSegements[i])))
                {
                    char[] trimChars = { '{', '}' };
                    string key = r.UrlPath[i].Trim(trimChars);

                    pathParams[key] = urlSegements[i];
                    continue;
                }

                return (false, c);
            }


            if (urlSegements[i] != r.UrlPath[i])
            {
                return (false, c);
            }
        }

        Dictionary<string, string> queryParams = GetQueryParams(c);

        c.PathParams = pathParams;
        c.QueryParams = queryParams;


        return (true, c);
    }


    private async Task SwytchRouter(RequestContext c)
    {
        string url = c.Request.Url.AbsolutePath;

        foreach (Route r in _registeredRoutes)
        {
            var (matched, context) = MatchUrl(url, r, c);
            if (matched)
            {
                if (r.Methods.Contains(c.Request.HttpMethod))
                {
                    await r.RequestHandler(context);
                    return;
                }

                //return with method not allowed
                c.Response.Headers.Set(HttpRequestHeader.Allow,string.Join("",r.Methods));
                await MethodNotAllowed(c);
                return;
            }
        }

        await NotFound(c);
    }


    public Func<RequestContext, Task> GetSwytchRouter()
    {
        if (_swytchRouter == null)
        {
            Func<RequestContext, Task> handler = this.SwytchRouter;

            foreach (var m in _registeredMiddlewares)
            {
                handler = m + handler;
            }

            _swytchRouter = handler;
        }

        return _swytchRouter;
    }
}