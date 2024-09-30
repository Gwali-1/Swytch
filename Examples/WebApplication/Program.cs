using Microsoft.Extensions.Logging;
using Swytch.App;
using WebApplication.Actions.Auth;
using WebApplication.Actions.Views;


SwytchApp swytchApp = new SwytchApp();
ILoggerFactory loggerFactory = LoggerFactory.Create(b => b.AddConsole());

//actions
PageAction pageAction = new(loggerFactory);
BookCollectionActions bookCollectionActions = new(loggerFactory,swytchApp);
AuthenticationAction authenticationAction = new (loggerFactory);


//swytchApp.AddLogging();
swytchApp.AddAuthentication(authenticationAction.AuthenticateUser);
swytchApp.AddStaticServer();


//register routes
swytchApp.AddAction("GET", "/", pageAction.HomePage);
swytchApp.AddAction("GET", "/books", bookCollectionActions.ShowBookCollection);
swytchApp.AddAction("GET,POST", "/addBook", bookCollectionActions.AddBook);

await swytchApp.Listen();