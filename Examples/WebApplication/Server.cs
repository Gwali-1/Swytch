

using System.Security.Claims;
using Swytch.App;
using Swytch.Structures;
using WebApplication.Actions;

//actions
PageAction pageActionAction= new PageAction();
AuthenticationAction authenticationAction = new AuthenticationAction();


SwytchApp server = new SwytchApp();
server.AddLogging();
server.AddAuthentication(authenticationAction.AuthenticateUser);







//register routes
server.AddAction("GET","/", pageActionAction.HomePage);

await server.Listen("http://127.0.0.1:8080/");