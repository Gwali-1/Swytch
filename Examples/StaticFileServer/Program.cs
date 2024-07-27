using System.Net;
using Swytch.App;
using Swytch.Structures;
using Swytch.utilities;

var server = new SwytchApp();

server.AddMiddleWare(async c =>
{
    Console.WriteLine($" {c.Request.ProtocolVersion} {c.Request.HttpMethod} {c.Request.Url}");
});

//action method can be writen like this
Func<RequestContext, Task> evening = async c =>
{
    await Utilities.ServeFile(c,"goodevening.html",HttpStatusCode.OK);

};


server.AddAction("GET", "/", evening);

server.AddAction("GET","/travelling", async c =>
{
    await Utilities.ServeFile(c,"traveling.html",HttpStatusCode.OK);
});


await server.Listen("http://localhost:8080/");