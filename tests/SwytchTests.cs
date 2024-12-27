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
    private static readonly SwytchApp TestServer = new();
    private static int _counter = 0;
    private readonly ILogger<SwytchTests> _logger;
    private static readonly HttpClient HttpClient = new HttpClient();

    public SwytchTests()
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger<SwytchTests>();
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
    public async Task Test_Request_Url_Path_Should_Match_Registered_Path(string path, HttpStatusCode httpStatusCode, string body)
    {
            //Arrange
            TestServer.AddAction("GET", path,
                async c => { await ResponseUtility.WriteTextToStream(c, body, httpStatusCode); });

            //start the server on a different thread 
            if (_counter == 0)
            {
                try
                {
                    await TestHelpers.StartTestServer("http://localhost:6081/", TestServer);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception thrown when starting test server");
                }
            }

        _counter++;

        //Act 
        var requestUri = "http://localhost:6081" + path;
        var response = await HttpClient.GetAsync(requestUri);
        var resoonseBody = await response.Content.ReadAsStringAsync();

        //Assert
        Assert.Equal(body, resoonseBody);
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
    private async Task Test_Request_Should_Return_NotFound_ResponseCode_For_Unmatched_Paths(string requestPath, string registeredPath,
        string requestMethod ,
        HttpStatusCode responseCode)
    {
        TestServer.AddAction(requestMethod, registeredPath,
            async c => { await ResponseUtility.WriteTextToStream(c, "Hello test", responseCode); });


        //start the server on a different thread 
        if (_counter == 0)
        {
            try
            {
                await TestHelpers.StartTestServer("http://localhost:6081/", TestServer);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown when starting test server");
            }
        }
        _counter++;


        //Act 
        var requestUri = "http://localhost:6081" + requestPath;
        var response = await HttpClient.GetAsync(requestUri);

        //Assert
        Assert.Equal(responseCode, response.StatusCode);
    }
}
