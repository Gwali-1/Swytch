using System.Text.Json;
using Microsoft.Extensions.Logging;
using Swytch.App;
using Swytch.Extensions;
using Swytch.Structures;
using WebApplication.Models;

namespace WebApplication.Actions.Views;

public class BookCollectionActions

{
    private readonly SwytchApp _app;
    private readonly ILogger<BookCollectionActions> _logger;
    private List<BookModel> Books = [];

    public BookCollectionActions(ILoggerFactory loggerFactory, SwytchApp app)
    {
        _app = app;
        _logger = loggerFactory.CreateLogger<BookCollectionActions>();
    }

    public async Task ShowBookCollection(RequestContext context)
    {
        var p = context.QueryParams;
        Console.WriteLine(p.ToString());
        await _app.RenderTemplate(context, "BooksView", Books);
    }

    public async Task AddBook(RequestContext context)
    {
        if (context.Request.HttpMethod.Equals("POST"))
        {
            try
            {
                _logger.LogInformation("received post request to add a new book");
                // _logger.LogDebug("request bod => {body}", context.Request.ContentType);
                var formBody = context.ReadFormBody();
                var newBooK = new BookModel
                {
                    Title = formBody["title"],
                    Author = formBody["author"],
                    Genre = formBody["genre"],
                    PublicationYear = Int32.Parse(formBody["publicationYear"]),
                    Description = formBody["Description"],
                    Rating = Int32.Parse(formBody["rating"])
                };
                // var newBooK = context.ReadJsonBody<BookModel>();
                _logger.LogInformation("body:{body}", JsonSerializer.Serialize(newBooK));
                Books.Add(newBooK);

                await context.ToRedirect("/books", ["name=malone"]);
                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occured while adding new book");
                await _app.RenderTemplate<object>(context, "AddBook", null);
            }
        }

        await _app.RenderTemplate(context, "AddBook", Books);
    }
}