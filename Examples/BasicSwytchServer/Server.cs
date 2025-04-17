using System.Net;
using System.Security.Claims;
using Swytch.App;
using Swytch.Extensions;
using Swytch.Structures;


SwytchApp swytchApp = new SwytchApp();


swytchApp.AddMiddleWare(async (context) => { context.ContextBag["name"] = "namuel"; });


swytchApp.AddAction("GET", "/obal/", async (context) =>
{
    Console.WriteLine(context.ContextBag["name"]);
    await context.WriteHtmlToStream("<h1>Hello from swytch<h1>", HttpStatusCode.OK);
});




// //middlewares
// var logger = async (RequestContext ctx) =>
// {
//     Console.WriteLine($"{ctx.Request.HttpMethod}    {ctx.Request.Url?.AbsolutePath}    {DateTime.UtcNow}");
// };
//
// //handlers
//
// var home = async (RequestContext ctx) =>
// {
//     await ResponseUtility.WriteTextToStream(ctx, "<h1>HOME</h1>", HttpStatusCode.OK);
// };
//
// var profile = async (RequestContext ctx) =>
// {
//     await ResponseUtility.WriteTextToStream(ctx, "<h1>PROFILE</h1>", HttpStatusCode.OK);
// };
//
// var login = async (RequestContext ctx) =>
// {
//     await ResponseUtility.WriteTextToStream(ctx, "<h1>LOGIN</h1>", HttpStatusCode.OK);
// };
//
//
// server.AddMiddleWare(logger);
// server.AddAction("GET", "/", home);
// server.AddAction("GET", "/profile/", profile);
// server.AddAction("GET", "/login", login);

await swytchApp.Listen();