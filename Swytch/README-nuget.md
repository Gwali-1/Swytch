

![](https://github.com/Gwali-1/Swytch/blob/main/Swytch/Logos/logo-1.png?raw=true)
[![.NET](https://github.com/Gwali-1/Swytch/actions/workflows/dotnet_build.yml/badge.svg)](https://github.com/Gwali-1/Swytch/actions/workflows/dotnet_build.yml)

Swytch is a web framework written in Csharp. It is lightweight, fast and offers an alternative and refreshing
way to author web services like REST APIs, static sites and web applications.It provides an expressive routing API, built-in templating
with RazorLight, support for asynchronous job processing using Actors, and seamless database integration with Dapper.

Make the `swytch` and try it out!

**SOME FEATURES**

- **Fast and Lightweight**  - Designed for high performance with minimal overhead.
- **Minimal and Expressive Routing** – Easily define routes and handlers for your web application with a clean API.
- **Path Parameters** –  Extract parameters directly from the URL for dynamic routing.
- **Templating with RazorLight** – Supports Razor-based templating for server-side rendering of dynamic content.
- **Precompiled Templates** – Supports template pre-compilation for improved performance.
- **Built-in Lightweight ORM** – Includes Dapper for efficient data access and interaction with databases.
- **Actor-Based Asynchronous Jobs** – Execute background tasks and non-blocking job execution using the built in Swytch
  Actor pool(Actor system).
- **Middleware Support** – Extend request handling with custom middleware
- **Resilient Request Handling** – Exceptions occurring during a request are isolated to that request, preventing
  failures from affecting the entire application.

## 📦 Installation

Install **Swytch** via NuGet:

```sh
dotnet add package Swytch
```

## ⚡ Basic Swytch App

```csharp
//create a swytchapp
var app = new SwytchApp();

//set up route 
app.AddAction("GET", "/", async (context) => {

    context.ToOk("Hello from Swytch!");
    
});

//start app
await swytchApp.Listen(); 
```
Run the application and navigate to `http://localhost:8080/`.

#### Check out the [documentation](#) for more information
#### Check out the [repository on github](https://github.com/Gwali-1/Swytch) 

## 🤝 Contributing

Contributions are highly valued, whether it's proposing new features, suggesting improvements, or reporting bugs. Your
input helps make Swytch even better—feel free to submit a PR to the [github repo](#)! 🚀

## 🔗 Links

- **Documentation**: [Swytch Docs](#)
- **Website**: [Swytch Website](#)
- **GitHub Repository**: [Swytch on GitHub](https://github.com/Gwali-1/Swytch)
- **Twitter**: [Swytch on Twitter](#)
