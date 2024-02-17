## Swytch

An http router in c#.

Basically a class library that will provide  an interface to register user provided methods/logic against routes(`url paths`)
and match inbound http request with this info.
The router ie `Swytch` will basically work by intercepting  incoming requests , retrieve the url path from the `Http Context` object and match it against the set of registered routes.
If route is matched , the associated method is called and if not appropriate response is communicated to the client.

**Why**
Motivations for this project may  vary
- An http router project is currently my favourite kind of project to undertake in a language.
- This project is to focus on C# as a language and not a gigantic famous framework for building applications for the web or whatever... And hopefully i can make something useable enough to write personal Web projects in.

    > I personally hate that spinning up a quick, simple and lightweight Api or even a static website using c# has all roads pointing to ASP.Net core( or  something else).
    > That's not what I'm looking for when i want to write a simple web project for myself for the purpose of learning or whatever use case .This is the best i can do in terms of giving a reason, i just want a lighter alternative where 
    i do much of the implementing myself. I want to keep `Swytch lighweight` and at they same time equip it with just enough features to get things moving quick.
    > So yes ,explore i must because c#  seems interesting enough (yes it is ) which unfortunately is buried under the shadow of the amazing framework its composed of/ built with.
    > I should be able to write a  simple Api which exposes 2 endpoints without going that route. A simple Library or micro framework which gives me just enough to work with is enough
    > and  also this is mostly how i like to write personal projects. The option to use a feature rich , reliable framework will always be available, but not today!

- Why Not ?


### Features
- Basic required functionality is a class that exposes methods that allow you to register your http request handling methods a
against routes.
- A way to quickly start an http server that uses above configurations.
- Dependency Injection container to allow service registration and usage  in your request handlers
- Path parameters



**Todo**

* Revision of architecture, project structure ...
* set up project structure
* create `Swtych` router class with associated data structures and  require properties...
* definition of  public methods that
- allow registration of routes with methods
- allow registration of middleware
- exposes a method to start wrapped http server 
- implement `Swytch` routing method. 

...etc









