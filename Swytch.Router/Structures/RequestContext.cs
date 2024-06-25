using System.Net;

namespace Swytch.Router.Structures;

/*RequestContext class is a custom type that essentially wraps desired members of the HttpListenerContext class
which in this case is the HttpListenerRequest, HttpListenerResponse, the IPrincipal object which  represents the current
client whose request is being handled. Some additional members include the query abd path params properties which will hold all query and path parameters supplied
in the request*/
public class RequestContext
{
    public HttpListenerRequest Request { get; set; }
    public HttpListenerResponse Response { get; set; }
    public System.Security.Principal.IPrincipal? User { get; set; }
    public Dictionary<string, string> PathParams { get; set; }
    public Dictionary<string, string> QueryParams { get; set; }

    public RequestContext(HttpListenerContext c)
    {
        Request = c.Request;
        Response = c.Response;
        User = c.User;
        PathParams = new Dictionary<string, string>();
        QueryParams = new Dictionary<string, string>();
    }
}