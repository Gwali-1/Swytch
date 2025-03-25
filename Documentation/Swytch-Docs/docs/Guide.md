# Guide
This section explores Swytch's core features and how to use them effectively. You'll learn how to define
routes, write handler methods, use middleware, serve static files, and configure your application. 
Each topic is explained with examples to help you get started quickly.
Whether you're building a simple API or a full web app, this guide will walk you through everything
you need to know to make the most of Swytch.





## Routing 
Swytch routes incoming requests by identifying the HTTP method and matching it against registered routes.
It considers both the request method and the exact path—if no match is found, a 405 Method Not Allowed 
response is returned. Swytch also supports dynamic path parameters, which you can define using {name} in
the route path. These parameters capture values from the URL as strings, with no automatic type conversion. 
For example, a route like "/users/{id}" will match requests such as "/users/42", where "42" is passed as
a string. These captured values, along with all query parameters, are made available in the RequestContext
type as `PathParams` and `QueryParams`, allowing easy access to request data.


#### AddAction
Routes in Swytch are registered using the `AddAction` method, which defines how requests should be handled.
It takes three parameters:

1. **HTTP Methods** – A capitalized string representing one or more HTTP methods (e.g., `"GET"`, `"POST"`, or `"GET,POST"`).
2. **Request Path** – A path relative to the root (e.g., `"/users/{id}"`).
3. **Action Method** – A method that that processes an incoming request

>Note that routes are matched **exactly**, meaning `"/about/"` is
**not the same** as `"/about"`.

An action method, also known as a handler, is the function that processes an incoming request. 
It must match the `Action<Task, RequestContext>` delegate, meaning it accepts a `RequestContext` parameter
and returns a `Task`. Instead of using inline lambdas, Swytch allows structuring code differently by
defining handler methods in a class and passing them by reference. For example:

```csharp
public class UserController {
    public static async Task GetUser(RequestContext ctx) {
        // Handle request
    }
}

app.AddAction("GET", "/users/{id}", UserController.GetUser);
```  

This approach improves code organization by keeping route definitions clean while keeping handlers
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
app.AddAction("GET", "/submit", async (context) => {...}));
app.AddAction("POST", "/submit", async (context) => {...}));
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

Middleware in Swytch is simple and straightforward. Each middleware function is queued for execution 
before the request reaches the intended handler method. This allows you to modify the `RequestContext`,
inspect or transform the request payload, or enforce custom logic before the request is processed.

For example, Swytch's [AddAuthentication](#authentication) method works by registering an authentication function as
middleware. This ensures that authentication checks are performed before the request reaches the actual 
handler.

To add custom middleware, you use the `AddMiddleware` method, which takes a single parameter—a function 
matching the `Action<RequestContext,Task>` delegate. The order in which middlewares are registered 
determines the order in which they execute.

### Example

```csharp
app.AddMiddleware(async (context) => {
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

### How It Works
The `AddAuthentication` method takes a delegate of type `Action<RequestContext, Task<AuthResponse>>`,
referred to as the *AuthHandler*. This method can be simply described an asynchronous method
receives the `RequestContext` and must return an
`AuthResponse`, which determines whether the request is authenticated.

#### AuthResponse
The AuthResponse struct represents the outcome of authentication in Swytch. After executing your 
authentication logic, you return an instance of AuthResponse, setting IsAuthenticated to true or false 
based on whether the request is authenticated.

Additionally, you can attach a ClaimsPrincipal to the response, which contains any claims related to 
the authenticated user. These claims can be accessed downstream in your handler via context.User, 
which is of type ClaimsPrincipal.



When authentication is enabled, Swytch automatically queues the *AuthHandler* as middleware, meaning
it executes on every incoming request. If authentication succeeds, the `IsAuthenticated` flag is set to
`true`, allowing the request to proceed. If authentication fails, Swytch blocks the request before it 
reaches any handler, ensuring unauthorized requests never reach protected resources.

### Example

#### Add A AuthHandler

```csharp
app.AddAuthentication(async (context) => {
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

In this example, the authentication handler checks for an `Authorization` header and validates it against a known token. If the token is valid, the request is authenticated and can proceed; otherwise, it is blocked.

With authentication enabled, every request must pass this check before reaching a registered route handler. If authentication fails, the request will not be processed further.

#### Accessing Claims and Authentication Status in a Handler

```csharp
app.AddAction("GET", "/profile", async (context) => {
        var username = ctx.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
        await context.ToOK(new { message = $"Welcome, {username}!" });
    }
});

```
Note that there is no explicit authentication status check in the handler code. 
This is because Swytch performs the authentication validation internally—if a request fails authentication,
the handler method will not be executed. This ensures that only authenticated requests reach your handler.
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

When enabled through the `SwytchConfig` type while creating an instance of `SwytchApp`, a built-in static 
file handler is registered. This handler automatically intercepts and serves static files from
the `Statics` directory without requiring you to write a custom handler.

### How It Works

Once the static file server is enabled, any request to the path: `/swytchserver/static/{filename}`

will be intercepted, and the requested file will be retrieved from the `Statics` directory and served automatically.
The response will include a `Cache-Control` header, with the cache duration set to either the default 
value or the one specified in `SwytchConfig`.

This makes it easy to serve static files such as images, stylesheets, and JavaScript files without
manually handling the request.

### Example

#### Enabling the Static File Server

To enable the static file server, configure it when creating an instance of `SwytchApp`:

```csharp
var app = new SwytchApp(new SwytchConfig 
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





## Serving Templates


Swytch provides built-in support for dynamic content rendering using RazorLight. Templates are stored in the `Templates` directory, where Swytch automatically locates and renders them.

### Template Precompilation

Template precompilation can be configured using the `SwytchConfig` class.

- **Enabled:** Increases startup time slightly but ensures that all template page loads are instantaneous.
- **Disabled (Default):** Results in a relatively faster startup time, but the first load of each template page is slightly slower. Subsequent loads remain instantaneous.

### Methods for Rendering Templates

Swytch offers two methods for working with templates on the `SwytchApp` class:

#### `GenerateTemplate<T>(string key, T model)`

This method generates a template and returns the result as a string.

- **Parameters:**
    - `key` *(string)* – The filename of the template (without extension) located in the `Templates` directory.
    - `model` *(T)* – An optional model to be passed into the template for dynamic content rendering.

- **Example Usage:**

```csharp
var content = app.GenerateTemplate("welcome", new { Name = "John Doe" });
Console.WriteLine(content); 
```

This will generate a string content with the welcome.cshtml template with the Name property
dynamically inserted.

#### `RenderTemplate<T>(RequestContext context, string key, T? model)`

This method directly sends the rendered content as an HTTP response to the client.

- **Parameters:**
    - `context` *(RequestContext)* – The current request context.
    - `key` *(string)* – The filename of the template (without extension) located in the `Templates` directory.
    - `model` *(T?)* – An optional model to be passed into the template for rendering.

- **Example Usage:**

```csharp
app.AddAction("GET", "/welcome", async ctx => 
{
    await app.RenderTemplate(ctx, "welcome", new { Name = "John Doe" });
});
```

When a client requests /welcome, Swytch will render welcome.cshtml with the provided model and return 
the generated content as an HTTP response.


### writing template files and link to page for more information


## Response Extensions



## Templates



## Database 



## Actor Pool


## WriteHtmlToStream

Be sure to excape html content if they are  from users to avoid injection. the method does no escaping and
sends the html content as is
