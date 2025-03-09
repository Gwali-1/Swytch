using System.Data;
using Swytch.Structures;

namespace Swytch.App;

/// <summary>
/// In summary, this is a representation of your running application. It contains and exposes functionality that manages the application lifetime, ensure resilience, allow you to
/// configure your web application as a whole  etc.
///  
/// </summary>
public interface ISwytchApp
{
    /// <summary>
    /// Registers a method matching the handler signature as a middleware.The order in which the registration
    /// is done matters .
    /// </summary>
    /// <param name="middleware">The middleware method(any method that takes in RequestContext and returns a task)</param>
    void AddMiddleWare(Func<RequestContext, Task> middleware);

    /// <summary>
    /// Registers http methods,url and the handler method 
    /// </summary>
    /// <param name="methods">The allowed http methods that the handler should be called for</param>
    /// <param name="path">the url path that the handler should be called for</param>
    /// <param name="requestHandler">The request handler</param>
    void AddAction(string methods, string path, Func<RequestContext, Task> requestHandler);

    /// <summary>
    /// Registers a new database store to the collection
    /// </summary>
    /// <param name="connectionString">The connection string to your datastore</param>
    /// <param name="provider">The type of datastore</param>
    void AddDatastore(string connectionString, DatabaseProviders provider);

    /// <summary>
    /// Returns a connection to the database that can be used to execute commands.
    /// Make sure to close connection after use
    /// </summary>
    /// <param name="provider">The data store type you registered in the datastore collection</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    IDbConnection GetConnection(DatabaseProviders provider);

    /// <summary>
    /// Enable logging of incoming request to the console. Logged information follows the format
    /// [Timestamp] - Http Method - Absolute Path - Request Origin 
    /// </summary>
    void AddLogging();

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
    void AddAuthentication(AuthHandler authHandler);

    /// <summary>
    /// registers a request handler that listens for static file requests to path /swytchserver/static/{filename}  and servers them directly from the
    /// static directory. This handler will run regardless of authentication state. If you want files to be served only
    /// if request is authenticated , register a different handler that listens on a different path
    /// </summary>
    void AddStaticServer();

    /// <summary>
    /// The Listen method starts the server 
    /// </summary>
    /// <param name="addr">The address the server should listen for incoming requests.
    ///The address should be a URI prefix composed of a scheme ,host ,optional port ...
    /// Prefix must end a forward slash. eg  "http://localhost:8080/".
    /// <a href="https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-net-httplistener">read more</a>
    /// </param>
    ///ref/
    Task Listen(string addr = "http://127.0.0.1:8080/");

    /// <summary>
    /// This method returns the the internal implementation of the router used in swytch.
    /// It essentially returns the method used internally to handle routing and process the request pipeline
    /// and is provided to aid in testing the routing logic for the library author and contibutors.
    /// </summary>
    /// <returns>The Swytch router method</returns>
    Func<RequestContext, Task> GetSwytchRouter();

    /// <summary>
    /// Compiles and renders the provided template  using the specified model and returns the compiled result.
    /// </summary>
    /// <param name="key">Unique identifier for the template.In this case the filename minus the extension</param>
    /// <param name="model"> The data model to use when rendering the template</param>
    /// <typeparam name="T">Type of data model</typeparam>
    /// <returns></returns>
    Task<string> GenerateTemplate<T>(string key, T model);

    /// <summary>
    /// Compiles template using specified model and serves it as html response.
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="key">Unique identifier for the template.In this case the filename minus the extension</param>
    /// <param name="model">The data model to use when rendering the template</param>
    /// <typeparam name="T">Type of data model</typeparam>
    Task RenderTemplate<T>(RequestContext context, string key, T? model);

    /// <summary>
    /// Shuts down the Swytch server
    /// </summary>
    void Close();

    /// <summary>
    /// Causes this instance to stop receiving new incoming requests and terminates processing of all ongoing requests
    /// </summary>
    void Stop();
}