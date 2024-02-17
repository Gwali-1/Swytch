## Swytch

An http router in c#.

Basically we shall write a class library that will provide  an interface to register user provided methods/logic against routes(`url paths`)
and match inbound http request with this info.
The router ie `Swytch` will basically work by  intercepting  incoming requests , retrieve the url path from the `Http Context` object and match it against the set of registered routes.
If route is matched , the associated method is called and if not appropriate response is communicated to the client.

### Features
- Basic required functionality is a class that exposes methods that allow you to register your http request handling methods a
against routes.
- A way to quickly start an http server that uses above configurations.
- Dependency Injection container to allow service registration
- Path parameters



**Todo**

* Quick reading , revision of architectute and project structure 
* set up project structure
* create swtych router class with associated data structures and properties
* define public methods that
- allow registration of routes with methods
- allow registrstion of middleware
- exposes a method to start wrapped http server 
- implement a routing match method

..etc









