using System.Net;

namespace Swytch.Structures;


class RequestContext
{

    public HttpListenerRequest Request;
    public HttpListenerResponse Response;
    public System.Security.Principal.IPrincipal? User;
    public Dictionary<string, string>? PathParams ;
    public Dictionary<string, string>?  QueryParams ;

    public RequestContext(HttpListenerContext c)
    {
        Request = c.Request;
        Response = c.Response;
        User = c.User;
    }

}
