using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;

namespace Swytch.Structures;

/// <summary>
///RequestContext class is a custom type that essentially wraps desired members of the HttpListenerContext class
///which in this case is the HttpListenerRequest, HttpListenerResponse, the IPrincipal object which  represents the current
///client whose request is being handled. Some additional members include the query and path params properties which will hold all 
///query and path parameters supplied in the request.
/// Provide this as argument to your request handling methods as it contains information unique to the current request being handled. 
/// </summary>
public interface IRequestContext
{
    //props
    /// <summary>
    /// Represents the HTTP request information as an object.
    /// </summary>
    HttpListenerRequest Request { get; set; }

    /// <summary>
    /// Represents the HTTP response object.
    /// </summary>
    HttpListenerResponse Response { get; set; }

    /// <summary>
    /// Represents the client whose request is being handled in this context.
    /// </summary>
    ClaimsPrincipal? User { get; set; }

    /// <summary>
    /// Key-value pair of all the path params extracted for the matched route. 
    /// </summary>
    Dictionary<string, string> PathParams { get; set; }

    /// <summary>
    /// Key-value pairs of all the query paramters extracted for the matched route.
    /// </summary>
    Dictionary<string, string?> QueryParams { get; set; }

    /// <summary>
    /// A key-value store for sharing arbitrary data across the request lifecycle.
    /// Keys and values are both of type <see cref="object"/>, so it's recommended to use unique key types
    /// to avoid collisions.
    /// </summary>
    public Dictionary<Object, object> ContextBag { get; set; }

    /// <summary>
    /// Boolean field which indicates whether a request passed the authentication handler logic or not 
    /// </summary>
    Boolean IsAuthenticated { get; set; }


    //methods


    /// <summary>
    /// Reads the json formatted request body and returns as a string .
    /// ContentType header must be set to application/json or InvalidDataException is thrown
    /// </summary>
    /// <returns>json request string sent as a post request</returns>
    /// <exception cref="InvalidDataException"></exception>
    public string ReadJsonBody();


    /// <summary>
    /// Reads the json formatted request body and returns deserialization to type T.
    /// ContentType header must be set to application/json or InvalidDataException is thrown
    /// </summary>
    /// <typeparam name="T">Represent the type to deserialize the body to</typeparam>
    /// <returns>json request string sent as a post request</returns>
    /// <exception cref="InvalidDataException"></exception>
    public T? ReadJsonBody<T>();

    /// <summary>
    /// Reads and returns the request body content and returns it as it is.
    /// </summary>
    /// <returns>Raw request body content</returns>
    public string ReadRawBody();


    /// <summary>
    /// Read the form urlencoded request body.
    /// ContentType header must be set to application/x-www-form-urlencoded  or InvalidDataException is thrown
    /// </summary>
    /// <returns>NameValueCollection that contains form fields and their values</returns>
    /// <exception cref="InvalidDataException"></exception>
    public NameValueCollection ReadFormBody();
}