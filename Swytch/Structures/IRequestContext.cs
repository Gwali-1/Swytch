using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;

namespace Swytch.Structures;

public interface IRequestContext
{
    HttpListenerRequest Request { get; set; }
    HttpListenerResponse Response { get; set; }
    ClaimsPrincipal? User { get; set; }
    Dictionary<string, string> PathParams { get; set; }
    Dictionary<string, string> QueryParams { get; set; }
    Boolean IsAuthenticated { get; set; }

    public string ReadJsonBody();
    public NameValueCollection ReadFormBody();

}