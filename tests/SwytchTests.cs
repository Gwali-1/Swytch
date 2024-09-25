using System.Net;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Swytch.App;
using Swytch.Structures;
using Swytch.utilities;
using Xunit;

namespace tests;

public class SwytchTests
{
    private static SwytchApp testServer = new();
    private static int _counter = 0;
    private readonly ILogger<SwytchTests> _logger;
    private static HttpClient _httpClient;

    public SwytchTests()
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger<SwytchTests>();
        _httpClient = new HttpClient();
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
    public async Task TestRequestUrlPathMatchingForGetMethod(string path, HttpStatusCode httpStatusCode, string body)
    {
        using (HttpListener listener = new HttpListener())
        {
            //Arrange
            testServer.AddAction("GET", path,
                async c => { await Utilities.WriteTextToStream(c, body, httpStatusCode); });

            //start the server on a different thread 
            if (_counter == 0)
            {
                try
                {
                    await TestHelpers.StartTestServer("http://localhost:6081/", testServer);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception thrown when starting test server");
                }
            }
        }

        _counter++;

        //Act 
        var requestUri = "http://localhost:6081" + path;
        var response = await _httpClient.GetAsync(requestUri);
        var resoonseBody = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(body, resoonseBody);
        Assert.Equal(httpStatusCode, response.StatusCode);
    }


    [Theory]
    [InlineData("/home", "/home/", "GET", HttpStatusCode.NotFound)]
    private async Task TestRequestUrlPathAndRegisteredUrlPathMatching(string requestPath, string requestMethod,
        string registeredPath,
        HttpStatusCode responseCode)
    {
        testServer.AddAction(requestMethod, registeredPath,
            async c => { await Utilities.WriteTextToStream(c, "Hello test", responseCode); });


        //start the server on a different thread 
        if (_counter == 0)
        {
            try
            {
                await TestHelpers.StartTestServer("http://localhost:6081/", testServer);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown when starting test server");
            }
        }
        _counter++;


        //Act 
        var requestUri = "http://localhost:6081" + requestPath;
        var response = await _httpClient.GetAsync(requestUri);
        var resoonseBody = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(responseCode, response.StatusCode);
    }
}