using System.Text.Json;
using Microsoft.Extensions.Logging;
using Swytch.App;
using Swytch.Structures;
using WebApplication.Models;

namespace WebApplication.Actions.Views;

public class BookCollectionActions

{
    private readonly SwytchApp _app;
    private readonly ILogger<BookCollectionActions> _logger;

    public BookCollectionActions(ILoggerFactory loggerFactory, SwytchApp app)
    {
        _app = app;
        _logger = loggerFactory.CreateLogger<BookCollectionActions>();
    }

    public async Task ShowBookCollection(RequestContext context)
    {
        await _app.RenderTemplate<object>(context, "BooksView", null);
    }

    public async Task AddBook(RequestContext context)
    {
        if (context.Request.HttpMethod.Equals("POST"))
        {
            _logger.LogInformation("recieved post request on addbokk endpoint");
            // _logger.LogDebug("request bod => {body}", context.Request.ContentType);
            // var formBody = context.ReadFormBody();
            var body = context.ReadJsonBody<AddBookModel>();
            _logger.LogInformation("body:{body}",JsonSerializer.Serialize(body));

            // _logger.LogInformation("Book Title:{title}" +
            //                        "\nAuthor:{author}" +
            //                        "\nGenre:{genre}" +
            //                        "\nDescription:{des}", formBody["title"], formBody["author"], formBody["genre"],
            //     formBody["description"]);
        }

        await _app.RenderTemplate<object>(context, "AddBook", null);
    }
}