using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swytch.App;
using WebApplication.Actions.Auth;
using WebApplication.Actions.Views;


SwytchApp swytchApp = new SwytchApp();
IServiceCollection container = new ServiceCollection();
container.AddLogging(c =>
{
    c.AddConsole();
    c.SetMinimumLevel(LogLevel.Information);
});
container.AddSingleton<ISwytchApp>(swytchApp);

IServiceProvider serviceProvider = container.BuildServiceProvider();

//actions
PageAction pageAction = new(serviceProvider);
BookCollectionActions bookCollectionActions = new(serviceProvider);
AuthenticationAction authenticationAction = new (serviceProvider);





//swytchApp.AddLogging();
// swytchApp.AddAuthentication(authenticationAction.AuthenticateUser);
swytchApp.AddStaticServer();


//register routes
swytchApp.AddAction("GET", "/", pageAction.HomePage);
swytchApp.AddAction("GET", "/books", bookCollectionActions.ShowBookCollection);
swytchApp.AddAction("GET,POST", "/addBook", bookCollectionActions.AddBook);

await swytchApp.Listen();