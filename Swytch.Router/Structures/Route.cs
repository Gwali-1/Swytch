namespace Swytch.Structures;

using System.Net;


//The represents a Route object which contains
//Url path as a list of strings
//The http methods to match and call the requestHandler for
//And the http Handler which is a Delegate Which  takes in HttpListnerContext as argument and returns Task

class Route
{
    public string[] urlPath;
    public Func<HttpListenerContext, Task> requestHandler;
    public List<string> methods = new List<string>();

   public  Route(Func<HttpListenerContext, Task> handler, string[] path)
    {
        urlPath = path;
        requestHandler = handler;
    }
}
