using System.Net;
using Swytch.Router.Structures;
using Swytch.Router.utilities;
using Swytch.Router.Structures;
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


server.AddMiddleWare(logger);
server.AddAction("GET", "/", home);
server.AddAction("GET", "/profile/", profile);
server.AddAction("GET", "/login", login);

await server.Listen("http://127.0.0.1:8080/");