using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using System.Net;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using RazorLight;
using Swytch.Structures;
using Swytch.utilities;

namespace Swytch.App;

/// <summary>
/// The SwytchApp class is the entry point to the framework library. An instance of this type exposes public APIs for that allow you
/// to register your middlewares , request handling methods , start the server etc. There is an internal routing implementation that handles
/// calling middlewares in the order they were registered and calling the right methods for the right routes and http verbs.
/// Call Listen when done registering your handling logic and middlewares to start accepting and processing requests.
/// </summary>
public class SwytchApp
{
    //registered routes
    private readonly List<Route> _registeredRoutes = new List<Route>();

    private readonly Queue<Func<RequestContext, Task>> _registeredMiddlewares = new();
    private Func<RequestContext, Task>? _swytchRouter;
    private readonly RazorLightEngine? _engine;
    private readonly ILogger<SwytchApp> _logger;
    private bool _enableAuth;
    private bool _enableStatic;
    private readonly Dictionary<DatabaseProviders, string> _dataSources = new();
    private string TemplateLocation { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");


    public SwytchApp(string? templateDiretory = null)
    {
        //set up logger 
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger<SwytchApp>();
        TemplateLocation = templateDiretory ?? TemplateLocation;

        //register available templates
        if (!Directory.Exists(TemplateLocation)) return;
        _engine = new RazorLightEngineBuilder().UseFileSystemProject(TemplateLocation).UseMemoryCachingProvider()
            .Build();
    }


    /// <summary>
    /// Registers a method matching the handler signature as a middleware.The order in which the registration
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
    /// Registers a new database store to the collection
    /// </summary>
    /// <param name="connectionString">The connection string to your datastore</param>
    /// <param name="provider">The type of datastore</param>
    public void AddDatastore(string connectionString, DatabaseProviders provider)
    {
        _dataSources[provider] = connectionString;
    }

    /// <summary>
    /// Returns a connection to the database that can be used to execute commands.
    /// Make sure to close connection after use
    /// </summary>
    /// <param name="provider">The data store type you registered datastore</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    public IDbConnection GetConnection(DatabaseProviders provider)
    {
        bool found = _dataSources.TryGetValue(provider, out string? dsn);
        if (!found)
        {
            throw new KeyNotFoundException($"No database provider {provider} was registered");
        }

        return provider switch
        {
            DatabaseProviders.SqlServer => new SqlConnection(dsn),
            DatabaseProviders.MySql => new MySqlConnection(dsn),
            DatabaseProviders.PostgreSql => new NpgsqlConnection(dsn),
            DatabaseProviders.SQLite => new SQLiteConnection(dsn),
            DatabaseProviders.Oracle => new OracleConnection(dsn),
            _ => throw new NotSupportedException("The specified database provider is not supported."),
        };
    }

    /// <summary>
    /// Enable logging of incoming request to the console. Logged information follows the format
    /// [Timestamp] - Http Method - Absolute Path - Request Origin 
    /// </summary>
    public void AddLogging()
    {
        //format [Timestamp] [http method] [path] [ip address]
        _registeredMiddlewares.Enqueue(async c =>
        {
            await Task.Delay(0);
            _logger.LogInformation("[{Timestamp}] - {HTTP Method} - {URL} - {IP}", DateTime.UtcNow,
                c.Request.HttpMethod,
                c.Request.Url?.AbsolutePath, c.Request.RemoteEndPoint);
        });
    }

    /// <summary>
    /// Adds authentication middleware to your pipeline allowing you to determine if a request is authenticated
    /// or not before it hit your handlers. When this method is called ,authentication is enabled for the application.
    /// </summary>
    /// <param name="authHandler">
    /// The authentication handler logic to run on every request.
    /// Supply a method with the delegate signature AuthHandler , which is a method that takes in the RequestContext object and
    /// returns an auth response indicating if authentication passed and a claims principal object representing security context of a user with
    /// a set of claims you registered 
    /// </param>
    public void AddAuthentication(AuthHandler authHandler)
    {
        _enableAuth = true;
        //call users auth handler by queuing it in middlewares
        _registeredMiddlewares.Enqueue(async c =>
        {
            await Task.Delay(0);
            AuthResponse authresponse = await authHandler(c);
            if (!authresponse.IsAuthenticated)
            {
                return;
            }

            c.User = authresponse.ClaimsPrincipal;
            c.IsAuthenticated = authresponse.IsAuthenticated;
        });
    }

    /// <summary>
    /// registers a request handler that listens for static file requests with path /swytchserver/static/{filename}  and servers them directly from the
    /// static directory. This handler will run regardless of authentication state. If you want files to be served only
    /// if request is authenticated , register a different handler that listens on a different path
    /// </summary>
    public void AddStaticServer()
    {
        _enableStatic = true;
        Func<RequestContext, Task> fileServer = async c =>
        {
            _ = c.PathParams.TryGetValue("filename", out var filename);
            // c.Response.Headers.Set(HttpResponseHeader.CacheControl, "max-age=86400");
            await Utilities.ServeFile(c, filename ?? "NoFile", HttpStatusCode.OK);
        };

        AddAction("GET", "/swytchserver/static/{filename}", fileServer);
    }


    /// <summary>
    /// The Listen method starts the server 
    /// </summary>
    /// <param name="addr">The address the server should listen for incoming requests.
    ///The address should be a URI prefix composed of a scheme ,host ,optional port ...
    /// Prefix must end a forward slash. eg  "http://localhost:8080/".
    /// <a href="https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-net-httplistener">read more</a>
    /// </param>
    ///ref/
    public async Task Listen(string addr = "http://127.0.0.1:8080/")
    {
        try
        {
            HttpListener server = new HttpListener();
            server.Prefixes.Add(addr);
            server.Start();
            _logger.LogInformation("Server is live at {addr}", addr);

            while (true)
            {
                HttpListenerContext ctx = await server.GetContextAsync();
                Func<RequestContext, Task> hndler = GetSwytchRouter();
                _ = Task.Run(() => hndler(new RequestContext(_logger,ctx)));
            }
        }
        catch (ArgumentException)
        {
            _logger.LogError("Make sure address provided matches scheme http:// or https:// or ends with /");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Server Stopped Unexpectedly");
            throw;
        }
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
    /// Compiles and renders the provided template  using the specified model and returns the compiled result.
    /// </summary>
    /// <param name="key">Unique identifier for the template.In this case the filename minus the extension</param>
    /// <param name="model"> The data model to use when rendering the template</param>
    /// <typeparam name="T">Type of data model</typeparam>
    /// <returns></returns>
    public async Task<string> GenerateTemplate<T>(string key, T model)
    {
        string templateName = key + ".cshtml";
        if (_engine != null)
        {
            string compiledResult = await _engine.CompileRenderAsync(templateName, model);
            return compiledResult;
        }

        return "";
    }

    /// <summary>
    /// Compiles template using specified model and serves it as html response.
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="key">Unique identifier for the template.In this case the filename minus the extension</param>
    /// <param name="model">The data model to use when rendering the template</param>
    /// <typeparam name="T">Type of data model</typeparam>
    public async Task RenderTemplate<T>(RequestContext context, string key, T? model)
    {
        string result = await GenerateTemplate(key, model);
        await Utilities.WriteHtmlToStream(context, result, HttpStatusCode.OK);
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
        const string responseBody = "INTERNAL SERVER ERROR (500)";
        await Utilities.WriteTextToStream(requestContext, responseBody, HttpStatusCode.NotFound);
    }

    private static async Task MethodNotAllowed(RequestContext requestContext)
    {
        const string responseBody = "METHOD NOT ALLOWED (405)";
        await Utilities.WriteTextToStream(requestContext, responseBody, HttpStatusCode.MethodNotAllowed);
    }

    private static async Task Unauthorized(RequestContext requestContext)
    {
        const string responseBody = "UNAUTHORIZED (401)";
        await Utilities.WriteTextToStream(requestContext, responseBody, HttpStatusCode.Unauthorized);
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
        //check if user is authenticated if authentication is enabled 
        if (_enableAuth && !VerifyIfStaticRequest(c))
        {
            bool result = await VerifyAuthentication(c);
            if (!result) return;
        }

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
                        _logger.LogWarning(e.Message);
                        _logger.LogWarning(e.StackTrace);
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

    private async Task<bool> VerifyAuthentication(RequestContext c)
    {
        if (c.IsAuthenticated) return true;
        await Unauthorized(c);
        return false;
    }

    private bool VerifyIfStaticRequest(RequestContext c)
    {
        if (!_enableStatic) return false;

        var staticRoute = _registeredRoutes.Find(r => r.UrlPath.Contains("swytchserver"));
        if (staticRoute is null) return false;
        var (match, _) = MatchUrl(c.Request.Url.AbsolutePath, staticRoute, c);
        return match;
    }
}