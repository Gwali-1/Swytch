# Quickstart

Ready to get started? In this section we shall get an introduction to Swytch. Be sure 
to follow the [installation](Installation.md) to set up a project and install Swytch first.



## A Basic Swytch App

A Swytch application in its most minimal and basic form will look something like this: 

```csharp
using System.Net;
using Swytch.App;
using Swytch.Extensions;


SwytchApp swytchApp = new SwytchApp();

swytchApp.AddAction("GET","/", async (context) =>
{
    await context.WriteHtmlToStream("<h1>Hello from swytch<h1>", HttpStatusCode.OK);
});

await swytchApp.Listen();

```
> Replace the content of Server.cs with this code block.

_This is a simple Swytch application that starts a server that listens on the default URI prefix `http://127.0.0.1:8080/` and
returns an HTML response to all HTTP GET requests to the root path or `/`. For every other path it shall respond with a 
**404 Not Found** and request to the root path with any other method apart 
from HTTP GET will ge **405 Method Not Allowed**._

Now let's go over  what this code does line by line.

1. First, we specify all the namespaces which contains the methods and classes that serve our purpose.

2. Then we go ahead and create an instance on a swytchApp.

3. Next we configure our Swytch app to perform a specific action when we receive a GET request on the root path(`/`) using
the `AddAction` method. We shall refer to this as adding an action to our Swytch app. The method takes in 3 parameters. First is
the HTTP method(s) on which we should perform the action, the second is the path and the third is the action method/handler method
itself. You will find more information on what the action method is, how to write it and different ways of using it in
the [guide](Guide.md#addaction) section.

4. Finally, we start our server and wait for requests.


To run the application simply make sure you're in the project root and then execute `Dotnet run` in the terminal 
or just start it from your IDE with whatever button provided and navigate to `http://127.0.0.1:8080/` in your browser.




## SwytchApp

`SwytchApp` represents the running application. An instance of this class is your web application or server.
It holds all configurations, including route definitions, server settings, static file caching
policies etc.

You can configure your `SwytchApp` using the [SwytchConfig](#) class, which provides sensible defaults. 
For example, the default configuration for the above application has cache max age is `3600` seconds,
and template precompilation is disabled (`false`). But we can change that with.

```csharp
SwytchApp swytchApp  = new SwytchApp(new SwytchConfig
{
    StaticCacheMaxAge = 7200, // Set cache max age to 2 hours
    PrecompileTemplates = true // Enable precompilation
});
```

For more details on available configurations, check the [Guide](#).





## Routing 

Swytch configures routing through the `AddAction` method, where you define the HTTP method(s),
the route path, and the handler method. Note that routes are matched **exactly**, meaning `"/about/"` is
**not the same** as `"/about"`.

The route registration in the above application specifies just one HTTP method,but we can be fancier 
and allow  multiple HTTP methods on the same route by separating them with a comma:

#### Multiple HTTP Methods

```csharp
swytchApp.AddAction("GET,POST", "/", async (context) => {...});
```
This ensures that both `GET` and `POST` requests to `"/"` are handled by the same action/handler method.

#### Route Matching Order

Routes are **matched in the order they are registered**. If you register the same path separately for different HTTP methods, 
the first one will always take precedence.

For example:

```csharp
app.AddAction("GET", "/submit", async (context) => {...}));
app.AddAction("POST", "/submit", async (context) => {...}));
```  

In this case, a `POST` request to `"/about"` will still match the first `GET` route,
resulting in a **405 Method Not Allowed** response.

To prevent this, define both HTTP methods to be handled by a single `AddAction` and make a decision how you handle
a request based on what HTTP method it came with:

```csharp
app.AddAction("GET,POST", "/submit",  async (context) => {

if (context.Request.HttpMethod == "POST")
{
   //handle POST request
   return;
}
//handle GET request
await context.ServeFile("SubmitPage.html",HttpStatusCode.OK);
    
});
```  
By doing this, both `GET` and `POST` requests will correctly match the intended handler.




## Response Extensions

Swytch provides handy extension methods on the `RequestContext` type to quickly send HTTP responses 
like  `WriteHtmlToStream` and `ServeFile` used in the code samples above.
You can find these methods in the `Swytch.Extensions` namespace.

For a full list of response methods and utilites currently available, check the [request context extensions page](RequestContext.md).



## Starting the Application

After configuring your  instance of `SwytchApp`, you finally call the `Listen` method and start the application. By default, it listens on port `8080`:

```csharp
await app.Listen();
```  

You can specify a different URL prefix if needed:

```csharp
await app.Listen("http://localhost:5000/");
```  

**Note:** The URL prefix **must** end with a trailing `/`, or an [ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenerprefixcollection.add?view=net-9.0#system-net-httplistenerprefixcollection-add(system-string)) will be thrown.

Also, always `await` this call, if not, the server will start and immediately exit.  


