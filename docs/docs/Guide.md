# Guide
This section explores Swytch's core features and how to use them effectively. You'll learn how to define
routes, write handler methods, use middleware, serve static files, configure your application etc. 
Each topic is explained with examples to help you get started quickly.
Whether you're building a simple API, a full web app or even a static site, this section will expose to you everything
you need to know to make the most of Swytch.




## Routing 
Swytch routes incoming requests by identifying its HTTP method and request path, and matching it against already registered routes.
It considers both the HTTP request method and the exact path that came with the request. If no match is found, a **405 Method Not Allowed** 
response is returned. Swytch also supports dynamic path parameters, which you can define using curly braces like this `{name}` in
the route path when registering or adding an action to your application. These parameters capture values from the URL as strings, with no automatic type conversion. 
For example, a route like `/users/{id}` will match requests paths such as `/users/42`, where `42` is parsed, retrieved and
provided(as the value of `id`) to you as a string. These captured path values, along with all query parameters, are made available 
in the `RequestContext` type in the  `PathParams` and `QueryParams` collection properties, allowing easy access.


#### AddAction
Routes in Swytch are registered using the `AddAction` method, which defines how requests should be handled.
It takes three parameters:

1. **HTTP Methods** – A capitalized string representing one or more HTTP methods (e.g., `"GET"`, `"POST"`, or `"GET,POST"`).
2. **Request Path** – A path relative to the root (e.g., `"/users/{id}"`).
3. **Action Method** – A method that can processes an incoming request.

>Note that routes are matched **exactly**, meaning `"/about/"` is
**not the same** as `"/about"`.

An action method, also known as a handler, is a method that processes an incoming request. For a method to be valid and qualify to
process an incoming request, it must match the `Action<Task, RequestContext>` delegate, meaning it accepts a `RequestContext`type parameter
and returns a `Task`. Note that instead inline lambdas like we have been doing all this while, we can also structure the code 
differently by defining handler methods in a class and passing them by reference because remember a valid action method just has to
satisfy an `Action<Task, RequestContext>` delegate . For example:

```csharp
public class UserController {
    public static async Task GetUser(RequestContext context) {
        // Handle request
    }
}
//usage 
var  userController = new  UserController();
swytchApp.AddAction("GET", "/users/{id}", userController.GetUser);
```  

This approach offers an alternative code organization approach by keeping route definitions clean while keeping handlers
separate.


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
swytchApp.AddAction("GET", "/submit", async (context) => {...}));
swytchApp.AddAction("POST", "/submit", async (context) => {...}));
```  

In this case, a `POST` request to `"/about"` will still match the first `GET` route,
resulting in a **405 Method Not Allowed** response.

To prevent this, define both methods in a single `AddAction` and make a decision how you handle
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

## Middleware

Middleware concept in Swytch is simple and straightforward. Each middleware is queued for execution 
before the request reaches the intended handler method. This allows you to do cool stuff like  modify the `RequestContext`,
inspect, transform the request payload or enforce custom logic before the request is processed by the main handler.

For example, Swytch's [AddAuthentication](#authentication) method works by registering a middleware that will execute a user provided
authentication method. This ensures that authentication checks are performed before the request reaches the actual 
handler.

To add custom middleware, you use the `AddMiddleware` method, which takes a single parameter, a method 
matching the `Action<RequestContext,Task>` delegate. **The order in which middlewares are registered 
determines the order in which they execute.**

### Example

```csharp
swytchApp.AddMiddleware(async (context) => {
    context.Response.Headers["X-Custom-Header"] = "Middleware Added";
});
```  

In this example, the middleware adds a custom header to every response before the request reaches its
handler. Since middlewares execute in the order they are registered, later middlewares or the final 
handler can rely on modifications made by earlier ones.




## Authentication

Swytch provides a mechanism to add authentication across your entire application or leave it completely
open. Authentication is added using the `AddAuthentication` method on an instance of `SwytchApp`.

By default, authentication is disabled. Calling `AddAuthentication` enables authentication, requiring 
each request to pass an authentication check before reaching its intended handler.

### How It Works?
The `AddAuthentication` method takes a delegate of type `Action<RequestContext, Task<AuthResponse>>`,
referred to as the *AuthHandler*. This method can be simply described an asynchronous method
receives the `RequestContext` and must return an
`AuthResponse`, which determines whether the request is authenticated or not.

#### AuthResponse
The AuthResponse struct represents the outcome of a request authentication in Swytch.Your 
authentication logic should return an instance of AuthResponse, setting the `IsAuthenticated` property to true or false 
based on whether the request is authenticated.

Additionally, you can attach a ClaimsPrincipal to the AuthResponse instance using the `ClaimsPrincipal` property 
which contains any claims related to 
the authenticated user. These claims can be accessed downstream in your handler via `context.User`, 
which is of type ClaimsPrincipal.



When authentication is enabled, Swytch automatically queues the *AuthHandler* as middleware, meaning
it executes on every incoming request. If authentication succeeds, the `IsAuthenticated` flag is set to
`true`, allowing the request to proceed. If authentication fails, Swytch blocks the request before it 
reaches any handler, ensuring unauthorized requests never reach protected resources.

### Example

#### Add A AuthHandler

```csharp
swytchApp.AddAuthentication(async (context) => {
    var token = context.Request.Headers["Authorization"];

    // Simulate an async token validation
    await Task.Delay(50);  

    if (token == "valid-token")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "JohnDoe"),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        var principal = new ClaimsPrincipal(identity);

        return new AuthResponse
        {
            IsAuthenticated = true,
            ClaimsPrincipal = principal
        };
    }

    return new AuthResponse { IsAuthenticated = false };
});

```  

In this example, the authentication handler checks for an `Authorization` header and validates it against a known token.
If the token is valid, the request is authenticated and can proceed, otherwise it is blocked.
With authentication enabled, every request must pass this check before reaching a registered route handler.
If authentication fails, the request will not be processed further.

#### Accessing Claims and Authentication Status in a Handler

```csharp
swytchApp.AddAction("GET", "/profile", async (context) => {
        var username = ctx.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
        await context.ToOK(new { message = $"Welcome, {username}!" });
    }
});

```
Note that there is no explicit authentication status check in the handler code. 
This is because Swytch performs the authentication validation internally.If a request fails authentication,
the handler method will not even be executed. This ensures that only authenticated requests reach your handler.
Within your handler, you can directly access any claims set in the AuthResponse.

## Serving Static Content

Swytch provides an extension method on `RequestContext` to serve static files from the `Statics` directory.
This directory must be located in the **root of the application's execution path**, meaning it should be
placed in the same directory where your application is running.

The method takes in two parameters:

1. **File name**  – The name of the file, including its extension.
2. **HTTP status code** – The response status code, using `System.Net.HttpStatusCode`.

If the specified file is not found in the `Statics` directory, a `FileNotFoundException` is thrown, caught, and logged. 
Swytch then responds with a `404 Not Found` status code and the message `"NOT FOUND (404)"`.

### Example
```csharp
await context.ServeFile("logo.png", HttpStatusCode.OK);
```

## Swytch Static File Server

When enabled through the `SwytchConfig` type when creating an instance of `SwytchApp`, a built-in static 
file handler is registered automatically for you. This handler intercepts and serves static files from
the `Statics` directory without requiring you to write a custom handler that does this.

### How It Works?

Once the static file server is enabled, any request to the path: `/swytchserver/static/{filename}`

will be intercepted, and the requested file will be retrieved from the `Statics` directory and served automatically.
The response will include a `Cache-Control` header, with the cache duration set to either the default 
value or the one specified in `SwytchConfig`.

This is very useful because it makes it easy to serve static files such as images, stylesheets, and JavaScript files without
manually handling the request.Let's look at an example.

### Example

#### Enabling the Static File Server

To enable the static file server, configure it when creating an instance of `SwytchApp`:

```csharp
var swytchApp = new SwytchApp(new SwytchConfig 
{
    EnableStaticFileServer = true,
    StaticFileCacheMaxAge = 7200 // Cache files for 2 hours
});
```


#### Using Static File Server in HTML

```html
<img src="/swytchserver/static/logo.png" alt="Logo">

```
This allows your Swytch application to serve static files seamlessly, making it practical for
hosting images, scripts, and stylesheets without additional configuration.





## Serving Dynamic  Templates


Swytch provides built-in support for dynamic content rendering using [RazorLight](https://github.com/toddams/RazorLight).
Templates are stored in the `Templates` directory, where Swytch automatically locates and renders them.

### Template Precompilation
Template precompilation can be configured using the `SwytchConfig` class.When 

- **Enabled:** Increases startup time relatively but ensures that all template page loads are instantaneous.
- **Disabled (Default):** Results in a relatively faster startup time, but the first request that renders a template page is 
 slower. However, all other subsequent loads will be  instantaneous.

### Methods for Rendering Templates

Swytch offers two methods for working with templates on the `SwytchApp` class:

### GenerateTemplate<T>(string key, T model)

This method generates a template and returns the result as a string.

- **Parameters:**
    - `key` *(string)* – The filename of the template (without extension) located in the `Templates` directory.
    - `model` *(T)* – A model to be passed into the template for dynamic content rendering.Pass null if no dynamic data 
  need to be inserted.
    - 
### Template File Naming and Directory

All template files must be placed inside a directory named `Templates` located in the root of your application.  
Each template should have the `.cshtml` extension.

For example, a template named `welcome` must be saved as `/Templates/welcome.cshtml`

### Template Content with Razor Syntax

Inside the `welcome.cshtml` file, you can write standard Razor syntax to bind and display data passed to it.

```cshtml
<h1>Welcome, @Model.Name!</h1>
<p>We’re glad to have you here.</p>
```
> You can check out the official Razor syntax documentation to learn how to write and structure your template files using [Razor](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-9.0)

You can render the above template from a route handler like indicated in the examples:

- **Example Usage:**

```csharp
var content = swytchApp.GenerateTemplate("welcome", new { Name = "John Doe" });
Console.WriteLine(content); 
```

This will generate a string content with the welcome.cshtml template with the Name property
dynamically inserted.

### RenderTemplate<T>(RequestContext context, string key, T? model)

This method works like the `GenerateTemplate` method but instead of returning the generated contect , it 
directly sends it as an HTTP response to the client.

- **Parameters:**
    - `context` *(RequestContext)* – The current request context.
    - `key` *(string)* – The filename of the template (without extension) located in the `Templates` directory.
    - `model` *(T)* – A model to be passed into the template for rendering.

**Example Usage:**

```csharp
swytchApp.AddAction("GET", "/welcome", async ctx => 
{
    await swytchApp.RenderTemplate(ctx, "welcome", new { Name = "John Doe" });
});
```

When a client requests `/welcome`, Swytch will fetch the contexts of `welcome.cshml`, bind it with the provided model and return 
the generated content as an HTTP response.



## Database 

### Working with Databases

Swytch includes built-in support for relational databases through the powerful and lightweight
[Dapper](https://github.com/DapperLib/Dapper) micro-ORM. Dapper was chosen for its simplicity, lightweight footprint, 
and easy-to-use APIs, making it a great fit for Swytch’s minimalistic approach.
Dapper allows you to execute SQL queries directly while taking care of parameter
mapping, making it ideal for building data-driven applications without the overhead of a full ORM.

### Registering a Database Source

To begin working with a database in Swytch, you must first register it using the `AddDatastore` method available on your
`ISwytchApp`(SwychApp) instance. This method stores the connection string along with the type of database provider, so Swytch can 
later retrieve and create a proper connection when needed.

The `AddDatastore` method accepts two parameters:

- `connectionString` (string): The connection string used to connect to the database.
- `provider` (`DatabaseProviders` enum): The type of database provider being registered.

Swytch currently supports the following database providers through the `Swytch.Structures.DatabaseProviders` enum:

- `DatabaseProviders.SqlServer` – _Microsoft SQL Server_
- `DatabaseProviders.MySql` – _MySQL_
- `DatabaseProviders.PostgreSql` – _PostgreSQL_
- `DatabaseProviders.SQLite` – _SQLite_
- `DatabaseProviders.Oracle` – _Oracle_

When you call `AddDatastore`, Swytch will associate the provided connection string with the selected database provider
internally. This registration must occur before attempting to use that provider or perform any database operations 
in your app logic.

### Retrieving a Connection

Once a datastore has been registered, you can obtain a connection to it using the `GetConnection` method. This method is also 
defined on the `ISwytchApp` interface and is your main entry point for executing SQL commands via Dapper.

The `GetConnection` method takes one parameter:

- `provider` (`DatabaseProviders`): The type of database you want to connect to.

It returns a raw connection object (e.g., `SqlConnection`, `NpgsqlConnection`, etc.) depending on the database provider you
registered earlier.

Some important notes:

- If you call `GetConnection` with a provider that has **not** been registered using `AddDatastore`, a `KeyNotFoundException` 
will be thrown.

- The returned connection **implements `IDisposable`**, meaning you should **always dispose of it** after use to avoid connection 
leaks.

You can dispose of the connection using:

- A `using` block (`using var conn = ...`)
- Or calling `conn.Dispose()` manually

### Example

Here's an example showing how to register a PostgreSQL database and use it to retrieve a list of users.

```csharp
// Register the PostgreSQL database
swytchApp.AddDatastore(
    "Host=localhost;Port=5432;Database=mydb;Username=myuser;Password=mypassword;",
    DatabaseProviders.PostgreSql
);

// Define a route that retrieves data from the database
swytchApp.AddAction("GET", "/users", async context =>
{
    using var conn = swytchApp.GetConnection(DatabaseProviders.PostgreSql);
    
    var users = await conn.QueryAsync<User>("SELECT * FROM users");

    await context.ToOk(users);
});

```



