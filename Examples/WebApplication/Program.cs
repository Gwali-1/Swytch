using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swytch.App;
using Swytch.Structures;
using Swytch.utilities;
using WebApplication.Actions.Views;
using WebApplication.Actors;

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

//Actors
ActorPool.InitializeActorPool(serviceProvider);
ActorPool.Register<TalkingActor>();



//Actions
PageAction pageAction = new(serviceProvider);
BookCollectionActions bookCollectionActions = new(serviceProvider);
// AuthenticationAction authenticationAction = new (serviceProvider);





swytchApp.AddLogging();
//swytchApp.AddAuthentication(authenticationAction.AuthenticateUser);
// swytchApp.AddStaticServer();




//Register actions
swytchApp.AddAction(RequestMethod.GET, "/", pageAction.HomePage);
swytchApp.AddAction(RequestMethod.GET, "/books", bookCollectionActions.ShowBookCollection);
swytchApp.AddAction(new RequestMethod[] { RequestMethod.GET, RequestMethod.POST }, "/addBook", bookCollectionActions.AddBook);

await swytchApp.Listen();