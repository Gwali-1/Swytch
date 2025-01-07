using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Logging;
using Swytch.App;
using Swytch.utilities;

namespace Swytch.Structures;

/*RequestContext class is a custom type that essentially wraps desired members of the HttpListenerContext class
which in this case is the HttpListenerRequest, HttpListenerResponse, the ClaimsPrincipal object which  represents the current
client whose request is being handled. Some additional members include the query and path params properties which will hold all query and path parameters supplied
in the request*/

/// <summary>
///RequestContext class is a custom type that essentially wraps desired members of the HttpListenerContext class
///which in this case is the HttpListenerRequest, HttpListenerResponse, the IPrincipal object which  represents the current
///client whose request is being handled. Some additional members include the query abd path params properties which will hold all 
///query and path parameters supplied in the request.
/// Provide this as argument to your request handling methods as it contains information unique to the current request being handled. 
/// </summary>
public class RequestContext : IRequestContext
{
    public HttpListenerRequest Request { get; set; }
    public HttpListenerResponse Response { get; set; }
    public ClaimsPrincipal? User { get; set; }
    public Dictionary<string, string> PathParams { get; set; }
    public Dictionary<string, string?> QueryParams { get; set; }
    public Boolean IsAuthenticated { get; set; }
    private readonly ILogger<SwytchApp> _logger;

    public RequestContext(ILogger<SwytchApp> logger, HttpListenerContext c)
    {
        Request = c.Request;
        Response = c.Response;
        PathParams = new Dictionary<string, string>();
        QueryParams = new Dictionary<string, string?>();
        IsAuthenticated = false;
        _logger = logger;
    }


    /// <summary>
    /// Reads the json formatted request body and returns as a string .
    /// ContentType header must be set to application/json or InvalidDataException is thrown
    /// </summary>
    /// <returns>json request string sent as a post request</returns>
    /// <exception cref="InvalidDataException"></exception>
    public string ReadJsonBody()
    {
        if (Request.ContentType != null && !Request.ContentType.Equals("application/json"))
        {
            throw new InvalidDataException("ContentType not application/json");
        }

        try
        {
            using StreamReader reader = new StreamReader(Request.InputStream, Request.ContentEncoding);
            string jsonBody = reader.ReadToEnd();
            return jsonBody;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.Message);
            throw;
        }
    }


    /// <summary>
    /// Reads the json formatted request body and returns deserialization to type T.
    /// ContentType header must be set to application/json or InvalidDataException is thrown
    /// </summary>
    /// <typeparam name="T">Represent the type to deserialize the body to</typeparam>

    /// <returns>json request string sent as a post request</returns>
    /// <exception cref="InvalidDataException"></exception>
    public T? ReadJsonBody<T>()
    {
        if (Request.ContentType != null && !Request.ContentType.Equals("application/json"))
        {
            throw new InvalidDataException("ContentType not application/json");
        }

        try
        {
            using StreamReader reader = new StreamReader(Request.InputStream, Request.ContentEncoding);
            string jsonBody = reader.ReadToEnd();

            var result = JsonSerializer.Deserialize<T>(jsonBody);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.Message);
            throw;
        }
    }
    
    /// <summary>
    /// Reads and returns the request body content and returns it as it is.
    /// </summary>
    /// <returns>Raw request body content</returns>

    public string ReadRawBody()
    {
        try
        {
            using StreamReader reader = new StreamReader(Request.InputStream, Request.ContentEncoding);
            string rawBody = reader.ReadToEnd();
            return rawBody;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.Message);
            throw;
        }
    }


    /// <summary>
    /// Read the form urlencoded request body.
    /// ContentType header must be set to application/x-www-form-urlencoded  or InvalidDataException is thrown
    /// </summary>
    /// <returns>NameValueCollection that contains form fields and their values</returns>
    /// <exception cref="InvalidDataException"></exception>
    public NameValueCollection ReadFormBody()
    {
        if (Request.ContentType != null && !Request.ContentType.Equals("application/x-www-form-urlencoded"))
        {
            throw new InvalidDataException("application/x-www-form-urlencoded");
        }

        try
        {
            using StreamReader reader = new StreamReader(Request.InputStream, Request.ContentEncoding);
            string formBody = reader.ReadToEnd();
            NameValueCollection parsedBody = HttpUtility.ParseQueryString(formBody);
            return parsedBody;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.Message);
            throw;
        }
    }


}