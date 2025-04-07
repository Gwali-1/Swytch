### RequestContext: A Detailed Overview

The `RequestContext` class in Swytch is designed to encapsulate all relevant information about an HTTP request being handled,
providing convenient access to details of the request and the response. Here is an overview of the key properties and their roles:

>RequestContext class is a custom type that essentially wraps desired members of the HttpListenerContext class
which in this case is the HttpListenerRequest, HttpListenerResponse, the ClaimsPrincipal object which  represents the current
client whose request is being handled. Some additional members include the query and path params properties which will hold all query and path parameters supplied
in the request

- **`HttpListenerRequest Request`**  
  This property holds the incoming HTTP request. It contains essential information about the request made by the client, 
  including the HTTP method (such as GET, POST, etc.), headers, and the body content. You can use this property to access raw 
  data about the client's request and handle it appropriately.

- **`HttpListenerResponse Response`**  
  The `Response` property represents the HTTP response that will be sent back to the client. With this, you can modify 
  the response by setting the status code, adding headers, and writing the response body. This is essential for shaping what 
  the client will receive after the request is processed.

- **`ClaimsPrincipal? User`**  
  The `User` property represents the client who made the request. It is an instance of `ClaimsPrincipal` and contains 
  information about the user's identity, including any claims they may have (such as roles, permissions, or other authentication data). 
  If authentication has failed, this property can be `null`, indicating there is no authenticated user. This is particularly useful for handling authentication and authorization in your application.

- **`Dictionary<string, string> PathParams`**  
  The `PathParams` property holds a collection of path parameters extracted from the URL. For instance, if the route is `/user/{id}`,
  and the incoming request is `/user/123`, this dictionary will contain the pair `{"id": "123"}`. 
  These parameters are crucial for handling dynamic parts of a route, allowing your application to adapt to different requests based on the URL structure.

- **`Dictionary<string, string?> QueryParams`**  
  This property stores the query parameters from the URL's query string. For example, a request to `/search?query=test&page=2` 
  will populate the `QueryParams` dictionary with the values `{"query": "test", "page": "2"}`. This makes it easy to work with
  parameters passed in the query string, which are often used to filter or paginate results in web applications.

- **`Boolean IsAuthenticated`**  
  The `IsAuthenticated` property is a boolean flag indicating whether the request has passed the authentication logic. 
  If this value is `true`, it means the request has been authenticated successfully; if it is `false`, the request has not been
  authenticated. This property is useful when you need to ensure that a request is valid and from an authenticated user before 
  processing it further.

### Why is `RequestContext` Important?

The `RequestContext` is crucial because it bundles all the essential information about the HTTP request and the response. T
his makes it easy for your application to interact with both the request and the response without needing to repeatedly access lower-level data
structures. Since most response extension methods are provided through `RequestContext`, it serves as the central point for working with HTTP
requests and responses.
By passing the `RequestContext` to your handler methods, you ensure that each method can access the relevant details specific to the
current request, such as parameters, headers, and authentication status. This not only simplifies request handling but also makes
your code more maintainable and modular.

### Why is `RequestContext` Important?

- It encapsulates all critical information about the HTTP request and response, ensuring that your application has easy access to the data it needs to process requests and generate appropriate responses.
- Most response extension methods are available through `RequestContext` because it gives direct access to the `HttpListenerResponse` object, allowing for seamless interaction with the response stream (e.g., writing data back to the client).
- By passing `RequestContext` to your handler methods, you ensure that each method can work with the specific details of the current request, without the need for repetitive code or global state management. This makes the request handling process clean and maintainable.




### Instance Methods in `RequestContext`

The `RequestContext` class provides several important instance methods that allow you to easily process incoming request data, 
making it simpler for your application to handle various types of HTTP requests. These methods help you interact with the request
body and extract data in formats such as JSON, raw text, or form-encoded data. Below are the key methods provided by the `RequestContext` instance:

- **`ReadJsonBody()`**  
  This method reads the JSON-formatted body of the HTTP request and returns it as a string. It expects the `Content-Type` header to be set to `application/json`. If the content type does not match, an `InvalidDataException` is thrown. This method is useful when the request body is expected to be in JSON format and you need to process it as a raw string.

- **`ReadJsonBody<T>()`**  
  Similar to the `ReadJsonBody()` method, this one reads the JSON-formatted body of the HTTP request but deserializes it directly into the specified type `T`. The `Content-Type` header must be set to `application/json`. This method is convenient when you want to work with strongly-typed objects and parse the request body into a specific class or data structure.

- **`ReadRawBody()`**  
  This method reads the raw content of the request body as a string, without any parsing or transformation. It is ideal when you want to handle arbitrary data formats or need to process the body without any assumptions about its structure. This method provides a flexible way to retrieve the entire content as-is.

- **`ReadFormBody()`**  
  This method reads the request body when the `Content-Type` header is set to `application/x-www-form-urlencoded`. It returns the body content as a `NameValueCollection`, which contains the form fields and their associated values. This is particularly useful for handling traditional form submissions, where data is encoded as key-value pairs.

These instance methods are part of the `RequestContext` class because they operate on the HTTP request's body. Since `RequestContext` contains all the necessary data about the incoming request, these methods provide an easy way to read and process the request body in various formats.


### Response Extensions in `RequestContext`

The `RequestContext` class provides several **response extension methods** that allow you to easily send HTTP responses back to the client. These methods are useful because they provide an easy interface to write to the response stream, set appropriate status codes, and format the response content as needed. The methods are designed to work seamlessly with the `RequestContext` since it already holds all the necessary information about the current request.

- **`WriteTextToStream(this RequestContext context, string payload, HttpStatusCode status)`**  
  This method writes a plain text response with the specified payload and status code. You can use it to return string-based content, such as error messages or success messages, to the client.

- **`WriteHtmlToStream(this RequestContext context, string payload, HttpStatusCode status)`**  
  Similar to the text response, this method writes an HTML response to the client. You can pass HTML content as a string,
  and it will be returned with the appropriate status code. This is useful for returning web pages or HTML-based content.
  Be sure to excape html content if they are  from users to avoid injection. the method does no escaping and
  sends the html content as is

- **`WriteJsonToStream<T>(this T @object, RequestContext context, HttpStatusCode status)`**  
  This method serializes an object of type `T` into a JSON format and writes it to the response body. It also sets the status code for the response. This is typically used for API responses where JSON is the expected format.

- **`ServeFile(this RequestContext context, string filename, HttpStatusCode status)`**  
  This method serves static files from the server. It reads a file from the `/statics` directory (inside the application's base directory) and streams it to the client. This is useful for serving assets like images, CSS files, JavaScript files, etc.

- **`ToOk<T>(this RequestContext context, T payload)`**  
  This method returns an HTTP response with a status code of `200 OK` and the provided payload. It's often used for successful responses in APIs where you return data in the body of the response.

- **`ToCreated<T>(this RequestContext context, T payload)`**  
  This method responds with a `201 Created` status code and the provided payload. It's typically used in RESTful APIs when a resource has been successfully created.

- **`ToAccepted<T>(this RequestContext context, T payload)`**  
  This method returns a `202 Accepted` status code, indicating that the request has been accepted for processing, but the actual processing may not be completed yet. It includes a payload, which may contain additional information or the state of the request.

- **`ToRedirect(this RequestContext context, string path, List<string>? queryParams = null)`**  
  This method issues a `302 Found` redirect response, redirecting the client to the specified path. Optionally, query parameters can be included in the redirect URL.

- **`ToPermanentRedirect(this RequestContext context, string path, List<string>? queryParams = null)`**  
  Similar to `ToRedirect()`, but this method issues a `301 Moved Permanently` response, indicating that the resource has permanently moved to the new location specified in the `path`.

- **`ToBadRequest<T>(this RequestContext context, T payload)`**  
  This method returns a `400 Bad Request` status code, typically used when the client sends invalid data. The `payload` provides more context about the error.

- **`ToUnauthorized<T>(this RequestContext context, T payload)`**  
  This method sends a `401 Unauthorized` status code, used when authentication is required but not provided or invalid. The `payload` contains information about the authentication failure.

- **`ToForbidden<T>(this RequestContext context, T payload)`**  
  This method sends a `403 Forbidden` status code, indicating that the client is authenticated but does not have permission to access the requested resource.

- **`ToNotFound<T>(this RequestContext context, T payload)`**  
  This method sends a `404 Not Found` status code when the requested resource cannot be found. The `payload` can contain details or a message explaining the error.

- **`ToInternalError(this RequestContext context, string message)`**  
  This method sends a `500 Internal Server Error` status code along with a meaningful message. It's typically used when there is an error on the server-side that prevents the request from being fulfilled.

- **`ToNotImplemented(this RequestContext context, string message)`**  
  This method returns a `501 Not Implemented` status code, indicating that the requested functionality is not supported by the server.

- **`ToResponse<T>(this RequestContext context, HttpStatusCode statusCode, T payload)`**  
  This method allows you to manually specify the HTTP status code and response data (`payload`). It's a flexible method for sending any type of response, with the status code and content of your choice.

These extension methods are provided to simplify the process of responding to HTTP requests by directly interacting with the response stream and setting appropriate HTTP status codes. By attaching them to the `RequestContext`, these methods have access to all necessary request data, allowing you to build responses efficiently and appropriately.