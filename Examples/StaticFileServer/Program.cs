using System.Net;
using Microsoft.Extensions.Logging;
using Swytch.App;
using Swytch.Structures;
using Swytch.utilities;

var server = new SwytchApp();

using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
ILogger logger = factory.CreateLogger<Program>();

server.AddMiddleWare(async c =>
{
    logger.LogInformation(" {c.Request.ProtocolVersion} {c.Request.HttpMethod} {c.Request.Url}",
        c.Request.ProtocolVersion, c.Request.HttpMethod, c.Request.Url.AbsolutePath);
});

//action method can be writen like this
Func<RequestContext, Task> evening = async c =>
{
    await Utilities.ServeFile(c, "goodevening.html", HttpStatusCode.OK);
};


//file server

Func<RequestContext, Task> fserver = async c =>
{
    string filename;
    _ = c.PathParams.TryGetValue("name", out filename);
    await Utilities.ServeFile(c, filename, HttpStatusCode.OK);
};

server.AddAction("GET", "/file/{name}", fserver);

server.AddAction("GET", "/", evening);

server.AddAction("GET", "/travelling",
    async c => { await Utilities.ServeFile(c, "traveling.html", HttpStatusCode.OK); });


await server.Listen("http://localhost:8080/");