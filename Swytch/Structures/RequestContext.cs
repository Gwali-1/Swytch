using System.Net;
using System.Security.Claims;

namespace Swytch.Structures;

/*RequestContext class is a custom type that essentially wraps desired members of the HttpListenerContext class
which in this case is the HttpListenerRequest, HttpListenerResponse, the IPrincipal object which  represents the current
client whose request is being handled. Some additional members include the query abd path params properties which will hold all query and path parameters supplied
in the request*/

/// <summary>
///RequestContext class is a custom type that essentially wraps desired members of the HttpListenerContext class
///which in this case is the HttpListenerRequest, HttpListenerResponse, the IPrincipal object which  represents the current
///client whose request is being handled. Some additional members include the query abd path params properties which will hold all 
///query and path parameters supplied in the request.
/// Provide this as argument to your request handling methods as it contains information unique to the current request being handled. 
/// </summary>
public class RequestContext
{
    public HttpListenerRequest Request { get; set; }
    public HttpListenerResponse Response { get; set; }
    public ClaimsPrincipal? User { get; set; }
    public Dictionary<string, string> PathParams { get; set; }
    public Dictionary<string, string> QueryParams { get; set; }

    public Boolean IsAuthenticated { get; set; }

    public RequestContext(HttpListenerContext c)
    {
        Request = c.Request;
        Response = c.Response;
        PathParams = new Dictionary<string, string>();
        QueryParams = new Dictionary<string, string>();
        IsAuthenticated = false;
    }
}

