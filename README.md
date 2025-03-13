<!-- LOGO -->

<p align="center">
 <img src="https://github.com/Gwali-1/Swytch/blob/main/Swytch/Logos/logo-1.png?raw=true" width=300 height=150>
</p>
<p align="center">
   Lightweight, Fast and Alternative.
    <br />
    <a href="#about">About</a>
    ·
    <a href="#">Documentation</a>
    ·
    <a href="#">Nuget</a>
</p>

[![.NET](https://github.com/Gwali-1/Swytch/actions/workflows/dotnet_build.yml/badge.svg)](https://github.com/Gwali-1/Swytch/actions/workflows/dotnet_build.yml)
[![Release](https://github.com/Gwali-1/Swytch/actions/workflows/release.yml/badge.svg)](https://github.com/Gwali-1/Swytch/actions/workflows/release.yml)

---

# About

Swytch is a web framework written in C#. It is lightweight, fast and offers an alternative and refreshing
way to author web services like REST APIs, web applications and static sites.It provides an expressive routing API, built-in
templating with RazorLight, support for asynchronous job
processing using Actors, and seamless database integration with Dapper.


> I started Swytch as an educational project to explore C# as a language and to experiment with a lightweight
> alternative
> for building simple web services or hacking on personal projects without the overhead of ASP.NET/ASP.NET CORE. It has
> been a
> rewarding experience, and I learned a lot along the way.
> I work on Swytch in my spare time, balancing it with my professional life and the other million things I have on my
> desk. It took over a year to get Swytch to a point where
> I felt it was stable enough to share. Since I only work on this when I can afford to, I kindly ask for patience when
> raising issues or reporting bugs. I'll address them as soon as I can.

#### Check out the [documentation](#) for more information

#### Check out the [devlogs and architectural notes ](https://github.com/Gwali-1/Swytch/blob/main/Notes/notes_26_06_24.md)

## Features

- **Minimal and Expressive Routing** – Define routes and handler methods easily with a clean API.
- **Path Parameters** – Extract parameters directly from the URL for dynamic routing.
- **Templating with RazorLight** – Supports Razor-based templating for rendering dynamic content.
- **Precompiled Templates** – Improves performance by precompiling Razor templates before execution.
- **Built-in Lightweight ORM** – Includes **Dapper** for efficient database interaction.
- **Asynchronous Job Processing** – Allows users to execute background and non-blocking tasks using **Actors**.
- **Resilient Request Handling** – Exceptions occurring during a request are **isolated to that request**, preventing
  failures from affecting the entire application.
- **Middleware Support** – Extend functionality by adding middleware for request/response processing.
- **Fast and Lightweight** – Designed for high performance with minimal overhead.

## 📦 Installation

---

Install **Swytch** via [NuGet](#):

```sh
 dotnet add package Swytch
```

Swytch supports **.NET 6+**.

## Basic Swytch Api

Create a basic **Swytch** application:

```csharp
//create a swytchapp
var app = new SwytchApp();

//set up route 
app.AddAction("GET", "/", async (context) => {
    context.ToOk("Welcome to Swytch!");
});

//start app
await swytchApp.Listen(); 
```

Run the application and navigate to `http://localhost:8080/`.

## Routing & Handlers


Define dynamic routes with path parameters:

```csharp
app.AddAction("GET","/users/{id}", async (context) => { 
    //get the id value
    var id = context.PathParams["id"];
});



app.AddAction("POST","/users/{id}", async (context) => { 
  //get the id value
   string userId;
   if(context.PathParams.TryGetValue("id", out userId)){
        //use here 
   }
});


//Register multiple HTTP methods to one handler
app.AddAction("GET,POST","/users/{id}", async (context) => { 
    //get the id value
    var id = context.PathParams["id"];
)};

```

## Middleware

Register **middleware**

```csharp
app.AddMiddleware(async (context) =>
{
    Console.WriteLine("Incoming request...");
});
```

## Templating with RazorLight


Use **RazorLight** to render dynamic template file:

```csharp
await app.RenderTemplate(context, "templateKey", Books);
```

## Background  Jobs (Actors)


Execute **background tasks** using **Actors**:

```csharp
//Initialize actor pool and register an actor
ActorPool.InitializeActorPool(serviceProvider);
ActorPool.Register<TalkingActor>();


//Execute task using actor
 ActorPool.Tell<TalkingActor,string>("Home");
```

## Database Integration (Dapper)

Query databases easily using **Dapper**:

```csharp
//Add data store 
swytchApp.AddDatastore("your_connection_string", DatabaseProviders.SQLite);

//execute query
 using IDbConnection dbConnection = app.GetConnection(DatabaseProviders.SQLite);
 var users = await dbConnection.QueryAsync<User>("SELECT * FROM Users");
```

## Contributing


Contributions are highly valued(seriously), whether it's proposing new features, suggesting improvements, or reporting bugs. Your
input helps make Swytch even better — feel free to submit a PR! 


