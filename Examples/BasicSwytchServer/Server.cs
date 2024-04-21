using System.Net;
using Swytch.Structures;
using Swytch.Utilies;
using swytch = Swytch.Router;

swytch.Swytch server = new swytch.Swytch();


//middlewares
var logger = async (RequestContext ctx) =>
{
    Console.WriteLine($"{ctx.Request.HttpMethod}    {ctx.Request.Url?.AbsolutePath}    {DateTime.UtcNow}");

};

//handlers

var home = async (RequestContext ctx) =>
{
    await Utilities.WriteStringToStream(ctx, "<h1>HOME</h1>", HttpStatusCode.OK);
};

var profile = async (RequestContext ctx) =>
{
    await Utilities.WriteStringToStream(ctx, "<h1>PROFILE</h1>", HttpStatusCode.OK);
};

var login = async (RequestContext ctx) =>
{
    await Utilities.WriteStringToStream(ctx, "<h1>LOGIN</h1>", HttpStatusCode.OK);
};


server.UseAsMiddleWare(logger);
server.MapRoute("GET", "/", home);
server.MapRoute("GET", "/profile", profile);
server.MapRoute("GET", "/login", login);

await server.Listen("http://127.0.0.1:8080/");