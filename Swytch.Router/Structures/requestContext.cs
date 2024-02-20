using System.Net;

namespace Swytch.Structures;


internal class RequestContext
{

    internal HttpListenerRequest Request;
    internal HttpListenerResponse Response;
    internal System.Security.Principal.IPrincipal? User;
    internal Dictionary<string, string>? PathParams;
    internal Dictionary<string, string>? QueryParams;

    internal RequestContext(HttpListenerContext c)
    {
        Request = c.Request;
        Response = c.Response;
        User = c.User;
    }

}
