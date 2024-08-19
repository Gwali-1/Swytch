using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Swytch.Structures;

namespace tests;

public static class TestHelpers
{
    public static Mock<IRequestContext> GetRequestContextInstance(string requestPath)
    {
        var request = (HttpListenerRequest)FormatterServices.GetUninitializedObject(typeof(HttpListenerRequest));
        var response = (HttpListenerResponse)FormatterServices.GetUninitializedObject(typeof(HttpListenerResponse));


        var uri = new Uri($"https://www.example.com/{requestPath}");

        //set url path
        request.SetPropertyValue("Url", uri, "uri");
        
        //set http method
        request.SetPropertyValue("HttpMethod", "GET","method");
        
        //set other fields 

        // var mockResponse = new Mock<HttpListenerResponse>();
        // // Set up the mockResponse to simulate certain behavior
        // mockResponse.Setup(r => r.StatusCode).Returns(200);
        //


        //mock request context , return objects of request and response for the properties 
        //create methods to set the htttp method, url for the request 
        var mockContext = new Mock<IRequestContext>();
        mockContext.Setup(c => c.Request).Returns(request);

        return mockContext;
    }


    static void SetPropertyValue<T>(this T obj, string propertyName, object value, string fn = "")
    {
        // Get the type of the object
        Type type = obj.GetType();

        // Get the property info
        PropertyInfo property = type.GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (property != null)
        {
            // Get the backing field for the property
            FieldInfo field = null;

            var fu = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var f in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (f.Name.Contains(fn.IsNullOrEmpty() ? propertyName : fn, StringComparison.OrdinalIgnoreCase))
                {
                    field = f;
                    break;
                }
            }

            if (field != null)
            {
                // Set the value of the backing field
                field.SetValue(obj, value);
            }
            else
            {
                Console.WriteLine($"Field not found for property '{propertyName}'");
            }
        }
    }
}