using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Swytch.Router.Structures;
using Swytch.Router.Structures;

namespace Swytch.Router.utilities;

internal static class Constant
{
    public static string Text { get; } = "text/html";
    public static string Json { get; } = "application/json";
    public static string StaticsDir { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Statics");
    public static string NotFound { get; } = "NOT FOUND(404";
}

public static class Utilities
{
    public static async Task WriteStringToStream(RequestContext context, string payload, HttpStatusCode status)
    {
        context.Response.StatusCode = (int)status;
        byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(payload);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constant.Text;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }


    public static async Task WriteJsonToStream<T>(this T @object, RequestContext context, HttpStatusCode status)
    {
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = Constant.Json;
        await using Stream writer = context.Response.OutputStream;
        await JsonSerializer.SerializeAsync(writer,
            @object); //asynchronously write to the output stream instead(of doing blocking serializing before
        //writing) 
    }


    public static async Task ServeFile(string filename, RequestContext context, HttpStatusCode status)
    {
        string filePath = Path.Combine(Constant.StaticsDir, filename);
        string contentType = Path.GetExtension(filePath) switch
        {
            ".html" => "text/html",
            _ => "text/plain",
        };
        int bufferSize = 4096; //4kb
        byte[] fileContent = new byte[bufferSize];
        try
        {
            await using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize, useAsync: true);
            int bytesRead;

            context.Response.ContentType = contentType;
            context.Response.StatusCode = (int)status;
            await using Stream writer = context.Response.OutputStream;
            while ((bytesRead = await fileStream.ReadAsync(fileContent, 0, fileContent.Length)) != 0)
            {
                context.Response.ContentLength64 = bytesRead;
                await writer.WriteAsync(fileContent);
            }
        }
        catch (FileNotFoundException)
        {
            await WriteStringToStream(context, Constant.NotFound, HttpStatusCode.NotFound);
        }
    }
}