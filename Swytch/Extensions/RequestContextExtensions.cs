using System.Net;
using System.Text.Json;
using Swytch.Structures;
using Swytch.utilities;

namespace Swytch.Extensions;

public static class RequestContextExtensions
{
    /// <summary>
    /// Writes a string as an http response and set the response status code to the one provided.
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="payload">The string payload to send</param>
    /// <param name="status">The response status code to set</param>
    public static async Task WriteTextToStream(this RequestContext context, string payload, HttpStatusCode status)
    {
        context.Response.StatusCode = (int)status;
        byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(payload);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constants.Json;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }

    /// <summary>
    /// Writes a html content as an http response and set the response status code to the one provided.
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="payload">The html content to send</param>
    /// <param name="status">The response status code to set</param>
    public static async Task WriteHtmlToStream(this RequestContext context, string payload, HttpStatusCode status)
    {
        context.Response.StatusCode = (int)status;
        byte[] responseBuffer = System.Text.Encoding.UTF8.GetBytes(payload);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constants.Html;
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
        context.Response.ContentType = Constants.Json;
        await using Stream writer = context.Response.OutputStream;
        await JsonSerializer.SerializeAsync(writer,
            @object); //asynchronously write to the output stream instead(of doing blocking serializing before
        //writing) 
    }

    /// <summary>
    /// Asynchronously reads and streams the contents of static files from the static directory({baseDirectory}/statics) of the application is running
    /// </summary>
    /// <param name="filename">The name of the file without the extension. eg catnames instead of catnames.txt</param>
    /// <param name="context">The current request context</param>
    /// <param name="status">The response status </param>
    public static async Task ServeFile(this RequestContext context, string filename, HttpStatusCode status)
    {
        string filePath = Path.Combine(Constants.StaticsDir, filename);
        string contentType = Path.GetExtension(filePath) switch
        {
            ".aac" => "audio/aac",
            ".abw" => "application/x-abiword",
            ".apng" => "image/apng",
            ".arc" => "application/x-freearc",
            ".avif" => "image/avif",
            ".avi" => "video/x-msvideo",
            ".azw" => "application/vnd.amazon.ebook",
            ".bin" => "application/octet-stream",
            ".bmp" => "image/bmp",
            ".bz" => "application/x-bzip",
            ".bz2" => "application/x-bzip2",
            ".cda" => "application/x-cdf",
            ".csh" => "application/x-csh",
            ".css" => "text/css",
            ".csv" => "text/csv",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".eot" => "application/vnd.ms-fontobject",
            ".epub" => "application/epub+zip",
            ".gz" => "application/gzip",
            ".gif" => "image/gif",
            ".htm" or ".html" => "text/html",
            ".ico" => "image/vnd.microsoft.icon",
            ".ics" => "text/calendar",
            ".jar" => "application/java-archive",
            ".jpeg" or ".jpg" => "image/jpeg",
            ".js" or ".mjs" => "text/javascript",
            ".json" => "application/json",
            ".jsonld" => "application/ld+json",
            ".mid" or ".midi" => "audio/midi",
            ".mp3" => "audio/mpeg",
            ".mp4" => "video/mp4",
            ".mpeg" => "video/mpeg",
            ".mpkg" => "application/vnd.apple.installer+xml",
            ".odp" => "application/vnd.oasis.opendocument.presentation",
            ".ods" => "application/vnd.oasis.opendocument.spreadsheet",
            ".odt" => "application/vnd.oasis.opendocument.text",
            ".oga" => "audio/ogg",
            ".ogv" => "video/ogg",
            ".ogx" => "application/ogg",
            ".opus" => "audio/ogg",
            ".otf" => "font/otf",
            ".png" => "image/png",
            ".pdf" => "application/pdf",
            ".php" => "application/x-httpd-php",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".rar" => "application/vnd.rar",
            ".rtf" => "application/rtf",
            ".sh" => "application/x-sh",
            ".svg" => "image/svg+xml",
            ".tar" => "application/x-tar",
            ".tif" or ".tiff" => "image/tiff",
            ".ts" => "video/mp2t",
            ".ttf" => "font/ttf",
            ".txt" => "text/plain",
            ".vsd" => "application/vnd.visio",
            ".wav" => "audio/wav",
            ".weba" => "audio/webm",
            ".webm" => "video/webm",
            ".webp" => "image/webp",
            ".woff" => "font/woff",
            ".woff2" => "font/woff2",
            ".xhtml" => "application/xhtml+xml",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".xml" => "application/xml",
            ".xul" => "application/vnd.mozilla.xul+xml",
            ".zip" => "application/zip",
            ".3gp" => "video/3gpp",
            ".3g2" => "video/3gpp2",
            ".7z" => "application/x-7z-compressed",
            _ => "application/octet-stream",
        };
        int bufferSize = 8192; //8kb
        byte[] fileContent = new byte[bufferSize];
        try
        {
            await using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize, useAsync: true);
            int bytesRead;

            context.Response.ContentType = contentType;
            context.Response.SendChunked = true;
            context.Response.StatusCode = (int)status;
            await using Stream writer = context.Response.OutputStream;
            while ((bytesRead = await fileStream.ReadAsync(fileContent, 0, fileContent.Length)) != 0)
            {
                await writer.WriteAsync(fileContent);
            }
        }
        catch (FileNotFoundException)
        {
            await WriteTextToStream(context, Constants.NotFound, HttpStatusCode.NotFound);
        }
    }


    /// <summary>
    /// Respond to HTTP request with status code 200 and payload
    /// </summary>
    /// <param name="context"> The current request context</param>
    /// <param name="payload">The response data,
    /// Acts as the wrapper for any data returned by the API call.</param>
    /// <typeparam name="T">Type of payload</typeparam>
    public static async Task ToOk<T>(this RequestContext context, T payload)
    {
        context.Response.StatusCode = (int)HttpStatusCode.OK;

        var responseModel = new ResponseStructureModel<T>("success", payload);
        var responseBuffer = JsonSerializer.SerializeToUtf8Bytes(responseModel);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constants.Json;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }

    /// <summary>
    /// Respond to HTTP request with status code 201 and payload
    /// 
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="payload">The response data.
    /// Acts as the wrapper for any data returned by the API call.</param>
    /// <typeparam name="T">Type of payload</typeparam>
    public static async Task ToCreated<T>(this RequestContext context, T payload)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Created;

        var responseModel = new ResponseStructureModel<T>("success", payload);
        var responseBuffer = JsonSerializer.SerializeToUtf8Bytes(responseModel);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constants.Json;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }

    /// <summary>
    /// Respond to HTTP request with status code 202 and payload
    /// 
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="payload">The response data.
    /// Acts as the wrapper for any data returned by the API call.</param>
    /// <typeparam name="T">Type of payload</typeparam>
    public static async Task ToAccepted<T>(this RequestContext context, T payload)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Accepted;

        var responseModel = new ResponseStructureModel<T>("success", payload);
        var responseBuffer = JsonSerializer.SerializeToUtf8Bytes(responseModel);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constants.Json;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }


    /// <summary>
    /// Redirect the user to another page by specifying the path.
    /// For For 302 Found.
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="path">The response data.
    /// Acts as the wrapper for any data returned by the API call.</param>
    /// <param name="queryParams">Query parameters to add to redirect path if available. eg ["name="malone",age=34]</param>
    public static async Task ToRedirect(this RequestContext context, string path, List<string>? queryParams = null)
    {
        var qParams = string.Empty;
        if (queryParams is not null && queryParams.Count > 0)
        {
            for (var i = 1; i < queryParams.Count; i++)
            {
                if (i == queryParams.Count)
                {
                    qParams += qParams[i];
                    break;
                }

                qParams += qParams[i] + "&";
            }

            path += "?" + qParams;
        }

        context.Response.StatusCode = (int)HttpStatusCode.Found;
        context.Response.RedirectLocation = path;
        context.Response.Close();
        await Task.Delay(0);
    }

    /// <summary>
    /// Redirect the user to another page of by specifying the path.
    /// For 301 Moved Permanently.
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="path">The response data.
    /// Acts as the wrapper for any data returned by the API call.</param>
    /// <param name="queryParams">Query parameters to add to redirect path if available</param>
    public static async Task ToPermanentRedirect(this RequestContext context, string path,
        List<string>? queryParams = null)
    {
        var qParams = string.Empty;
        if (queryParams is not null && queryParams.Count > 0)
        {
            for (var i = 1; i < queryParams.Count; i++)
            {
                if (i == queryParams.Count)
                {
                    qParams += qParams[i];
                    break;
                }

                qParams += qParams[i] + "&";
            }

            path += "?" + qParams;
        }

        context.Response.StatusCode = (int)HttpStatusCode.MovedPermanently;
        context.Response.RedirectLocation = path;
        context.Response.Close();
        await Task.Delay(0);
    }


    /// <summary>
    /// Respond to HTTP request with status code 400 and payload
    /// 
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="payload">The response data.
    /// Acts as the wrapper for any data returned by the API call.</param>
    /// <typeparam name="T">Type of payload</typeparam>
    public static async Task ToBadRequest<T>(this RequestContext context, T payload)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        var responseModel = new ResponseStructureModel<T>("fail", payload);
        var responseBuffer = JsonSerializer.SerializeToUtf8Bytes(responseModel);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constants.Json;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }


    /// <summary>
    /// Respond to HTTP request with status code 401 and payload
    /// 
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="payload">The response data.
    /// Acts as the wrapper for any data returned by the API call.</param>
    /// <typeparam name="T">Type of payload</typeparam>
    public static async Task ToUnauthorized<T>(this RequestContext context, T payload)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

        var responseModel = new ResponseStructureModel<T>("fail", payload);
        var responseBuffer = JsonSerializer.SerializeToUtf8Bytes(responseModel);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constants.Json;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }

    /// <summary>
    /// Respond to HTTP request with status code 403 and payload
    /// 
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="payload">The response data.
    /// Acts as the wrapper for any data returned by the API call.</param>
    /// <typeparam name="T">Type of payload</typeparam>
    public static async Task ToForbidden<T>(this RequestContext context, T payload)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        var responseModel = new ResponseStructureModel<T>("fail", payload);
        var responseBuffer = JsonSerializer.SerializeToUtf8Bytes(responseModel);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constants.Json;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }


    /// <summary>
    /// Respond to HTTP request with status code 404 and payload
    /// 
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="payload">The response data.
    /// Acts as the wrapper for any data returned by the API call.</param>
    /// <typeparam name="T">Type of payload</typeparam>
    public static async Task ToNotFound<T>(this RequestContext context, T payload)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;

        var responseModel = new ResponseStructureModel<T>("fail", payload);
        var responseBuffer = JsonSerializer.SerializeToUtf8Bytes(responseModel);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constants.Json;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }

    /// <summary>
    /// Respond to HTTP request with status code 500 and payload
    /// 
    /// </summary>
    /// <param name="context">The current request context.</param>
    /// <param name="message">A meaningful, end-user-readable (or at the least log-worthy) message, explaining what went wrong</param>
    public static async Task ToInternalError(this RequestContext context, string message)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var responseModel = new InternalErrorResponseModel("fail", message);
        var responseBuffer = JsonSerializer.SerializeToUtf8Bytes(responseModel);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constants.Json;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }


    /// <summary>
    /// Respond to HTTP request with status code 501 and payload
    /// 
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="message">A meaningful, end-user-readable (or at the least log-worthy) message, explaining what went wrong</param>
    public static async Task ToNotImplemented(this RequestContext context, string message)
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;

        var responseModel = new InternalErrorResponseModel("fail", message);
        var responseBuffer = JsonSerializer.SerializeToUtf8Bytes(responseModel);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constants.Json;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }


    /// <summary>
    /// Respond to HTTP request manually specifying status code and response datat 
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="payload">The response data</param>
    /// <param name="statusCode"></param>
    public static async Task ToResponse<T>(this RequestContext context, HttpStatusCode statusCode, T payload)
    {
        context.Response.StatusCode = (int)statusCode;

        var responseModel = new ResponseStructureModel<T>(statusCode.ToString(), payload);
        var responseBuffer = JsonSerializer.SerializeToUtf8Bytes(responseModel);
        context.Response.ContentLength64 = responseBuffer.Length;
        context.Response.ContentType = Constants.Json;
        await using System.IO.Stream writer = context.Response.OutputStream;
        await writer.WriteAsync(responseBuffer);
    }
}