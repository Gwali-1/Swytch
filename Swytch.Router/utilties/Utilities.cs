using System.Net;

namespace Swytch.Utilies;


public class ResponseWriter
{
    public ResponseWriter() { }

    public static void WriteStringToStream(HttpListenerContext context, string payload, HttpStatusCode status)
    {
        context.Response.StatusCode = (int)status;
        byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(payload);
        context.Response.ContentLength64 = responseBuffer.Length;
        using System.IO.Stream writer = context.Response.OutputStream;
        writer.Write(responseBuffer, 0, responseBuffer.Length);
    }

}
