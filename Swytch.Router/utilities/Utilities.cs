using System.Net;
using Swytch.Router.Structures;
using Swytch.Router.Structures;

namespace Swytch.Router.utilities;

public static class Utilities
{
    public static async Task WriteStringToStream(RequestContext context, string payload, HttpStatusCode status)
    {
        context.Response.StatusCode = (int)status;
        byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(payload);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = "text/html";
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }
    
    
    

}