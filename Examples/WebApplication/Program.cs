using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swytch.App;
using Swytch.Structures;
using WebApplication.Actions.Auth;
using WebApplication.Actions.Views;




SwytchApp swytchApp = new SwytchApp(new SwytchConfig
{
    StaticCacheMaxAge = "5",
    EnableStaticFileServer = true,
    PrecompileTemplates = true
});


//set up service container 
IServiceCollection container = new ServiceCollection();
//register services here 
container.AddLogging(c =>
{
    c.AddConsole();
    c.SetMinimumLevel(LogLevel.Information);
});
container.AddSingleton<ISwytchApp>(swytchApp);
IServiceProvider serviceProvider = container.BuildServiceProvider();


//Actions
PageAction pageAction = new(serviceProvider);
BookCollectionActions bookCollectionActions = new(serviceProvider);
// AuthenticationAction authenticationAction = new (serviceProvider);





swytchApp.AddLogging();
//swytchApp.AddAuthentication(authenticationAction.AuthenticateUser);
// swytchApp.AddStaticServer();




//Register actions
swytchApp.AddAction("GET", "/", pageAction.HomePage);
swytchApp.AddAction("GET", "/books", bookCollectionActions.ShowBookCollection);
swytchApp.AddAction("GET,POST", "/addBook", bookCollectionActions.AddBook);

await swytchApp.Listen();