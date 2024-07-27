using System.Collections.Specialized;
using System.Net;
using RazorLight;
using Swytch.Structures;
using Swytch.utilities;

namespace Swytch.App;

/// <summary>
/// The Swytch class is the entry point to the framework library. An instance of this type exposes public APIs for that allow you
/// to register your middlewares , request handling methods , start the server etc. There is an internal routing implementation that handles
/// calling middlewares in the order they were registered and calling the right methods for the right routes and http verbs.
/// call Listen when done registering your handling logic and middlewares to start accepting and processing requests.
/// </summary>
public class SwytchApp
{
    //registered routes
    private readonly List<Route> _registeredRoutes = new List<Route>();
    private readonly Queue<Func<RequestContext, Task>> _registeredMiddlewares = new();
    private Func<RequestContext, Task>? _swytchRouter;
    private readonly RazorLightEngine _engine;
    public string TemplateLocation { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");


    public SwytchApp(string? templateDiretory = null)
    {
        TemplateLocation = templateDiretory ?? TemplateLocation;
        _engine = new RazorLightEngineBuilder().UseFileSystemProject(TemplateLocation).UseMemoryCachingProvider()
            .Build();
    }


    //adds middleware in the order in which they were registered
    /// <summary>
    /// Registers a method matching the handler signature as a middleware. The order in which the registration
    /// is done matters .
    /// </summary>
    /// <param name="middleware">The middleware method(any method that takes in RequestContext and returns a task)</param>
    public void AddMiddleWare(Func<RequestContext, Task> middleware)
    {
        _registeredMiddlewares.Enqueue(middleware);
    }


    /// <summary>
    /// Registers http methods,url and the handler method 
    /// </summary>
    /// <param name="methods">The allowed http methods that the handler should be called for</param>
    /// <param name="path">the url path that the handler should be called for</param>
    /// <param name="requestHandler">The request handler</param>
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


    /// <summary>
    /// The Listen method starts the server 
    /// </summary>
    /// <param name="addr">The address the server should listen for incoming requests.
    ///The address should be a URI prefix composed of a scheme ,host ,optional port ...
    /// Prefix must end a forward slash. eg  http://localhost:8080/. read more https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-net-httplistener
    /// /// </param>
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
                _ = Task.Run(() => hndler(new RequestContext(ctx)));
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
        await Utilities.WriteTextToStream(requestContext, responseBody, HttpStatusCode.NotFound);
    }

    private static async Task InternalServerError(RequestContext requestContext)
    {
        string responseBody = "INTERNAL SERVER ERROR (500)";
        await Utilities.WriteTextToStream(requestContext, responseBody, HttpStatusCode.NotFound);
    }

    private static async Task MethodNotAllowed(RequestContext requestContext)
    {
        string responseBody = "METHOD NOT ALLOWED (405)";
        await Utilities.WriteTextToStream(requestContext, responseBody, HttpStatusCode.MethodNotAllowed);
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
            if (r.UrlPath[i].StartsWith('{') && r.UrlPath[i].EndsWith('}'))
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
        string? url = c.Request.Url?.AbsolutePath;

        foreach (Route r in _registeredRoutes)
        {
            var (matched, context) = MatchUrl(url ?? string.Empty, r, c);
            if (matched)
            {
                if (r.Methods.Contains(c.Request.HttpMethod))
                {
                    try
                    {
                        await r.RequestHandler(context);
                        return;
                    }
                    catch (Exception e)
                    {
                        await InternalServerError(context);
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                }

                //return with method not allowed
                c.Response.Headers.Set(HttpRequestHeader.Allow, string.Join("", r.Methods));
                await MethodNotAllowed(c);
                return;
            }
        }

        await NotFound(c);
    }


    /// <summary>
    /// This method returns the the internal implementation of the router used in swytch.
    /// It essentially returns the method used internally to handle routing and process the request pipeline
    /// and is provided to aid in testing the routing logic for the library author and contibutors.
    /// </summary>
    /// <returns>The Swytch router method</returns>
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

    /// <summary>
    /// Compiles and renders the provided template  using the specified model.
    /// </summary>
    /// <param name="key">Unique identifier for the template.In this case the filename minus the extension</param>
    /// <param name="model"> The data model to use when rendering the template</param>
    /// <typeparam name="T">Type of data model</typeparam>
    /// <returns></returns>
    public async Task<string> GenerateTemplate<T>(string key, T model)
    {
        string templateName = key + ".cshtml";
        string compiledResult = await _engine.CompileRenderAsync(templateName, model);
        return compiledResult;
    }

    /// <summary>
    /// Compiles template using specified model and serves it as html response.
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="key">Unique identifier for the template.In this case the filename minus the extension</param>
    /// <param name="model">The data model to use when rendering the template</param>
    /// <typeparam name="T">Type of data model</typeparam>
    public async Task RenderTemplate<T>(RequestContext context, string key, T model)
    {
        string result = await GenerateTemplate(key, model);
        await Utilities.WriteHtmlToStream(context, result, HttpStatusCode.OK);
    }
}