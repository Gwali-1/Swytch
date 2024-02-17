using System.Net;

namespace Swytch.Utilies;


public class ResponseWriter
{
    public ResponseWriter() { }

    public static void WriteStringToStream(HttpListenerContext context, string payload)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(payload);
        context.Response.ContentLength64 = responseBuffer.Length;
        using System.IO.Stream writer = context.Response.OutputStream;
        writer.Write(responseBuffer, 0, responseBuffer.Length);
    }

}
