namespace Swytch.Structures;

using System.Net;


//The represents a Route object which contains
//Url path as a list of strings
//The http methods to match and call the requestHandler for
//And the http Handler which is a Delegate Which  takes in HttpListnerContext as argument and returns Task

internal class Route
{
    internal string[] urlPath;
    internal Func<HttpListenerContext, Task> requestHandler;
    internal List<string> methods = new List<string>();

    internal Route(Func<HttpListenerContext, Task> handler, string[] path)
    {
        urlPath = path;
        requestHandler = handler;
    }
}
