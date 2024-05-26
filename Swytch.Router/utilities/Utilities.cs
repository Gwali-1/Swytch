using System.Net;
using System.Text.Json;
using Swytch.Router.Structures;
using Swytch.Router.Structures;

namespace Swytch.Router.utilities;

public static class Utilities
{
    public static string Text { get; } = "text/html";
    public static string Json { get; } = "application/json";

    public static async Task WriteStringToStream(RequestContext context, string payload, HttpStatusCode status)
    {
        context.Response.StatusCode = (int)status;
        byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(payload);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Text;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }


    public static async Task WriteJsonToStream<T>(this T @object, RequestContext context, HttpStatusCode status)
    {
        
        //check if it is  not an object/ class
        context.Response.StatusCode = (int)status;
        byte[] responseBuffer =
            @object is null 
                ? System.Text.Encoding.UTF8.GetBytes("{}")
                : System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize<T>(@object));
        context.Response.ContentType = Json;
        context.Response.ContentLength64 = responseBuffer.Length;
        await using Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }
}