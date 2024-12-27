using System.Collections.Specialized;
using System.Net;
using System.Security.Claims;

namespace Swytch.Structures;

public interface IRequestContext
{
    //props
    HttpListenerRequest Request { get; set; }
    HttpListenerResponse Response { get; set; }
    ClaimsPrincipal? User { get; set; }
    Dictionary<string, string> PathParams { get; set; }
    Dictionary<string, string> QueryParams { get; set; }
    Boolean IsAuthenticated { get; set; }


    //methods
    public string ReadJsonBody();
    public T? ReadJsonBody<T>();
    public string ReadRawBody();
    public NameValueCollection ReadFormBody();
}