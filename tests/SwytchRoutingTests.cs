using System.Net;
using Microsoft.Extensions.Logging;
using Swytch.App;
using Swytch.Structures;
using Swytch.utilities;
using Xunit;
using Xunit.Abstractions;

namespace tests;

public class SwytchRoutingTests : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly SwytchApp _testServer = new();
    private readonly ILogger<SwytchRoutingTests> _logger;
    private readonly HttpClient _httpClient = new HttpClient();

    public SwytchRoutingTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger<SwytchRoutingTests>();
    }


    [Theory]
    [InlineData("/", HttpStatusCode.OK, "home")]
    [InlineData("/profile/page", HttpStatusCode.OK, "profile")]
    [InlineData("/authentication", HttpStatusCode.OK, "auth")]
    [InlineData("/kingship", HttpStatusCode.OK, "king")]
    [InlineData("/fish/", HttpStatusCode.OK, "not a fan")]
    [InlineData("/fish/baloon/circus/without/", HttpStatusCode.OK, "About right")]
    [InlineData("/change/c/clown", HttpStatusCode.OK, "clown")]
    [InlineData("/fish///mode", HttpStatusCode.OK, "not a fan")]
    [InlineData("/fish/hbhbdbbndddfhgghsvhhvjhdbbbjdfbd/hvvjbshhhms", HttpStatusCode.OK, " orange")]
    [InlineData("/fish/bjhebmmnm,bjjsbjj/{{}/", HttpStatusCode.OK, "milk")]
    [InlineData("/fish/man/gold", HttpStatusCode.OK, "changelog")]
    [InlineData("/fish/blessd''''/ns/", HttpStatusCode.OK, "watermelons")]
    [InlineData("/fish/vdhvhsjjds/", HttpStatusCode.OK, "willing")]
    [InlineData("/fish/,sfbbjkgfjkewh*8/", HttpStatusCode.OK, "super")]
    [InlineData("/fish/,nkndnklhrel", HttpStatusCode.OK, "merlin")]
    [InlineData("/fish/__+==-4e/jhje", HttpStatusCode.OK, "group")]
    [InlineData("/fish/::::ejjje/", HttpStatusCode.OK, "changer")]
    [InlineData("/fish/uiijjr/87774/", HttpStatusCode.OK, "fresh and blessed ")]
    [InlineData("/fish/34566//445", HttpStatusCode.OK, "believe")]
    [InlineData("/fish/sjjhdh/", HttpStatusCode.OK, "true")]
    public async Task Test_Request_Url_Path_Should_Match_Registered_Path(string path, HttpStatusCode httpStatusCode,
        string body)
    {
        //ARRANGE
        _testServer.AddAction(new [] {RequestMethod.GET}, path,
            async c => { await ResponseUtility.WriteTextToStream(c, body, httpStatusCode); });

        //start the server instance on a different thread 
        try
        {
            _testOutputHelper.WriteLine("starting server");
            await TestHelpers.StartTestServer("http://localhost:6081/", _testServer);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception thrown when starting test server");
        }


        //ACT
        await Task.Delay(20);
        var requestUri = "http://localhost:6081" + path;
        var response = await _httpClient.GetAsync(requestUri);
        var responseBody = await response.Content.ReadAsStringAsync();

        //ASSERT
        Assert.Equal(body, responseBody);
        Assert.Equal(httpStatusCode, response.StatusCode);
    }


    [Theory]
    [InlineData("/home", "/home/", "GET", HttpStatusCode.NotFound)]
    [InlineData("/home/", "/home/'", "GET", HttpStatusCode.NotFound)]
    [InlineData("/home/.band", "/home/band", "GET", HttpStatusCode.NotFound)]
    [InlineData("/home/band", "/home/band/", "GET", HttpStatusCode.NotFound)]
    [InlineData("/home/crook", "/home/crook/brand", "GET", HttpStatusCode.NotFound)]
    [InlineData("/home/manona/cro", "/home/manona/cro", "GET", HttpStatusCode.NotFound)]
    [InlineData("/home/", "/home/.", "GET", HttpStatusCode.NotFound)]
    [InlineData("/home/ ", "/home/  ", "GET", HttpStatusCode.NotFound)]
    [InlineData("/home/", "/home/    ", "GET", HttpStatusCode.NotFound)]
    [InlineData("/home / ", "/home/", "GET", HttpStatusCode.NotFound)]
    [InlineData("/home/meaning", "/home/meaning/", "GET", HttpStatusCode.NotFound)]
    [InlineData("/branded/ ", "/ḅranded/", "GET", HttpStatusCode.NotFound)]
    public async Task Test_Request_Should_Return_NotFound_ResponseCode_For_Unmatched_Paths(string requestPath,
        string registeredPath,
        string requestMethod,
        HttpStatusCode responseCode)
    {
        //ARRANGE
        
        _testServer.AddAction(requestMethod, registeredPath,
            async c => { await ResponseUtility.WriteTextToStream(c, "Hello test", responseCode); });


        //start the server on a different thread 
        try
        {
            _testOutputHelper.WriteLine("starting server");
            await TestHelpers.StartTestServer("http://localhost:6081/", _testServer);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception thrown when starting test server");
        }


        
        
        //ACT 
        
        await Task.Delay(10);
        var requestUri = "http://localhost:6081" + requestPath;
        var response = await _httpClient.GetAsync(requestUri);
        
        

        //ASSERT
        
        Assert.Equal(responseCode, response.StatusCode);
    }



    [Theory]
    [InlineData( "/home/", "/home/" , RequestMethod.GET, HttpStatusCode.OK)]
    [InlineData( "/homeIndex/", "/home/" , RequestMethod.GET, HttpStatusCode.NotFound)]
    [InlineData( "/Send/", "/Send/" , RequestMethod.POST, HttpStatusCode.OK)]
    [InlineData( "/Del/", "/Delete/" , RequestMethod.DEL, HttpStatusCode.NotFound)]
    [InlineData( "/edit/", "/edit/" , RequestMethod.PUT, HttpStatusCode.OK)]
    [InlineData( "/update/", "/update/" , RequestMethod.PATCH, HttpStatusCode.OK)]
    [InlineData( "/heads/", "/heads/" , RequestMethod.HEAD, HttpStatusCode.OK)]

    public async Task Test_Request_Should_Return_As_What_Set_using_Request_Method_Enum(
        string requestPath,
        string registeredPath,
        RequestMethod method,
        HttpStatusCode responseCode)
    {
        
        _testServer.AddAction(method, registeredPath,
            async c => { await ResponseUtility.WriteTextToStream(c, "Hello test", responseCode); });


        //start the server on a different thread 
        try
        {
            _testOutputHelper.WriteLine("starting server");
            await TestHelpers.StartTestServer("http://localhost:6081/", _testServer);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception thrown when starting test server");
        }


        
        
        //ACT 
        
        await Task.Delay(10);
        var requestUri = "http://localhost:6081" + requestPath;

        HttpResponseMessage response = method switch
        {
            RequestMethod.GET => await _httpClient.GetAsync(requestUri),
            RequestMethod.POST => await _httpClient.PostAsync(requestUri, new StringContent("")),
            RequestMethod.DEL => await _httpClient.DeleteAsync(requestUri),
            RequestMethod.PUT => await _httpClient.PutAsync(requestUri, new StringContent("")),
            RequestMethod.PATCH => await _httpClient.PatchAsync(requestUri,new StringContent("")),
            RequestMethod.HEAD => await _httpClient.SendAsync(new HttpRequestMessage() { RequestUri= new Uri(requestUri) ,Method = HttpMethod.Head}),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };

        //var response =  await _httpClient.GetAsync(requestUri);

        
        Assert.Equal(responseCode, response.StatusCode);
    }
    public void Dispose()
    {
        _testServer.Stop();
        _testServer.Close();
        _httpClient.Dispose();
    }
    
    
    
}