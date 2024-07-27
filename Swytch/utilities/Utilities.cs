using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Swytch.Structures;

namespace Swytch.utilities;

internal static class Constant
{
    public static string Text { get; } = "text/plain";
    public static string Html { get; } = "text/html";
    public static string Json { get; } = "application/json";
    public static string StaticsDir { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Statics");
    public static string NotFound { get; } = "NOT FOUND(404";
}

/// <summary>
/// The Utilities class is meant to expose a set of helpful static methods that help perform certain tasks for convenience.
/// </summary>
public static class Utilities
{
    /// <summary>
    /// Writes a string as an http response setting to response status code to the one provided.
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="payload">The string payload to send</param>
    /// <param name="status">The response status code to set</param>
    public static async Task WriteTextToStream(RequestContext context, string payload, HttpStatusCode status)
    {
        context.Response.StatusCode = (int)status;
        byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(payload);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constant.Text;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }

    /// <summary>
    /// Writes a html content as an http response setting to response status code to the one provided.
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="payload">The html content to send</param>
    /// <param name="status">The response status code to set</param>
    public static async Task WriteHtmlToStream(RequestContext context, string payload, HttpStatusCode status)
    {
        context.Response.StatusCode = (int)status;
        byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(payload);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constant.Html;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }


    /// <summary>
    /// Asynchronously serializes and writes an object as http json response
    /// </summary>
    /// <param name="object">Object being serialized</param>
    /// <param name="context">The current request context</param>
    /// <param name="status">The response status code to set</param>
    /// <typeparam name="T">Generic type parameter</typeparam>
    public static async Task WriteJsonToStream<T>(this T @object, RequestContext context, HttpStatusCode status)
    {
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = Constant.Json;
        await using Stream writer = context.Response.OutputStream;
        await JsonSerializer.SerializeAsync(writer,
            @object); //asynchronously write to the output stream instead(of doing blocking serializing before
        //writing) 
    }

    /// <summary>
    /// Asynchronously reads and streams the contents of static files from the static directory in the base directroy in which the application is running
    /// </summary>
    /// <param name="filename">The name of the file without the extension. eg catnames instead of catnames.txt</param>
    /// <param name="context">The current request context</param>
    /// <param name="status">The response status </param>
    public static async Task ServeFile(RequestContext context,string filename, HttpStatusCode status)
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
            await WriteTextToStream(context, Constant.NotFound, HttpStatusCode.NotFound);
        }
    }
}