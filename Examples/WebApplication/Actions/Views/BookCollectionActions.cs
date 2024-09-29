using Swytch.App;
using Swytch.Structures;

namespace WebApplication.Actions.Views;

public class BookCollectionActions

{
    private readonly SwytchApp _app;

    public BookCollectionActions(SwytchApp app)
    {
        _app = app;
    }

    public async Task ShowBookCollection(RequestContext context)
    {
        await _app.RenderTemplate<object>(context, "BooksView", null);
    }

    public async Task AddBook(RequestContext context)
    {
        await _app.RenderTemplate<object>(context, "AddBook", null);
    }
}