using System.Net;
using Microsoft.Extensions.Logging;
using Swytch.App;
using Swytch.Structures;
using Swytch.utilities;

var server = new SwytchApp();
server.EnableLogging();  // enable or disable request logging

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