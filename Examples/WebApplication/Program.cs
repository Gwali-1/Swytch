using Swytch.App;
using WebApplication.Actions;
using WebApplication.Actions.Auth;
using WebApplication.Actions.Views;


SwytchApp swytchApp = new SwytchApp();

//actions
PageAction pageAction = new PageAction();
BookCollectionActions bookCollectionActions = new(swytchApp);
AuthenticationAction authenticationAction = new AuthenticationAction();


swytchApp.AddLogging();
swytchApp.AddAuthentication(authenticationAction.AuthenticateUser);
swytchApp.AddStaticServer();


//register routes
swytchApp.AddAction("GET", "/", pageAction.HomePage);
swytchApp.AddAction("GET", "/books", bookCollectionActions.ShowBookCollection);
swytchApp.AddAction("GET", "/addBook", bookCollectionActions.AddBook);

await swytchApp.Listen("http://127.0.0.1:8080/");