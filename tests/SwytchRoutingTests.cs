using System.Net;
using Microsoft.Extensions.Logging;
using Swytch.App;
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
        _testServer.AddAction("GET", path,
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
    
    public void Dispose()
    {
        _testServer.Stop();
        _testServer.Close();
        _httpClient.Dispose();
    }
    
    
    
}