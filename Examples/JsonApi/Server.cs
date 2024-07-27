using JsonApi.Actions;
using Swytch;
using Swytch.App;
using Swytch.Structures;


SwytchApp server = new SwytchApp();

//logging middleware
server.AddMiddleWare(async ctx =>
{
    await Task.Delay(0);
    Console.WriteLine($"{ctx.Request.HttpMethod}    {ctx.Request.Url?.AbsolutePath}    {DateTime.UtcNow}");
});

Cars cars = new Cars();
Fighters fighters = new Fighters();

server.AddAction("GET","/cars/", cars.All);
server.AddAction("GET","/car/{make}",cars.Get);

server.AddAction("GET","/fighters/", fighters.All);
server.AddAction("GET","/fighter/{name}",fighters.Get);


await server.Listen("http://127.0.0.1:8080/");

