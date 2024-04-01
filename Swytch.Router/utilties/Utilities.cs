using System.Net;
using Swytch.Structues;

namespace Swytch.Utilies;


public class Utilities
{
    public Utilities() { }

    public static async Task WriteStringToStream(RequestContext context, string payload, HttpStatusCode status)
    {
        context.Response.StatusCode = (int)status;
        byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(payload);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Respomse.ContentType = "text/html"
        using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer, 0, responseBuffer.Length);
    }


    //method to writem json to output stream
    //method to return html document 


}
