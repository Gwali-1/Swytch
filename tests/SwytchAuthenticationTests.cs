using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swytch.App;
using Swytch.Extensions;
using Swytch.utilities;
using Xunit;

namespace tests;

public class SwytchAuthenticationTests
{
    private readonly SwytchApp _testServer = new();
    private static int _counter = 0;
    private readonly ILogger<SwytchAuthenticationTests> _logger;
    private  readonly HttpClient _httpClient = new HttpClient();

    public SwytchAuthenticationTests()
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger<SwytchAuthenticationTests>();
    }


    [Fact]
    public async Task Test_Request_Should_Return_Correct_Response_For_Wrong_Basic_Credentials_When_Auth_Is_Disabled()
    {
        // arrange
        var path = "/protected/resource";
        var responseBody = "Hello from handler";
        var clientCredentials = "wrongId:wrongPassword";
        var expectedResponse = "Hello from handler";
        var responseCode = HttpStatusCode.OK;
        

        _testServer.AddAction("GET", path,
            async c => { await c.WriteTextToStream(responseBody, responseCode); });

        //start the server on a different thread 
        try
        {
            await TestHelpers.StartTestServer("http://localhost:6083/", _testServer);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception thrown when starting test server");
        }


        //add auth header
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientCredentials));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);


        //act
        var requestUri = "http://localhost:6083" + path;
       await Task.Delay(100);
        var response = await _httpClient.GetAsync(requestUri);
        var actualResponseBody = await response.Content.ReadAsStringAsync();


        //assert
        Assert.Equal(expectedResponse, actualResponseBody);
        Assert.Equal(responseCode, response.StatusCode);
    }


    //test auth handler correct
    [Theory]
    [InlineData("wrongId:wrongPassword", HttpStatusCode.Unauthorized,
        "UNAUTHORIZED (401)")]
    [InlineData("wrongId2:wrongPassword2", HttpStatusCode.Unauthorized,
        "UNAUTHORIZED (401)")]
    [InlineData("wrongId3:wrongPassword3", HttpStatusCode.Unauthorized,
        "UNAUTHORIZED (401)")]
    [InlineData("wrongPasswordonly", HttpStatusCode.Unauthorized,
        "UNAUTHORIZED (401)")]
    [InlineData("wrongNameOnly", HttpStatusCode.Unauthorized,
        "UNAUTHORIZED (401)")]
    [InlineData("", HttpStatusCode.Unauthorized,
        "UNAUTHORIZED (401)")]
    [InlineData("---", HttpStatusCode.Unauthorized,
        "UNAUTHORIZED (401)")]
    [InlineData("              ", HttpStatusCode.Unauthorized,
        "UNAUTHORIZED (401)")]
    [InlineData("w123345334:-334552", HttpStatusCode.Unauthorized,
        "UNAUTHORIZED (401)")]
    [InlineData("wrongId:wrongPassword:kbdjbbw ", HttpStatusCode.Unauthorized,
        "UNAUTHORIZED (401)")]
    [InlineData("correctMan124:Correxpassword12", HttpStatusCode.OK,
        "Hello from handler")]
    [InlineData("correctMan124:Correxpassword12:additional", HttpStatusCode.OK,
        "Hello from handler")]
    [InlineData("correctMan124:  Correxpassword12", HttpStatusCode.Unauthorized,
        "UNAUTHORIZED (401)")]
    [InlineData("   correctMan124: Correxpassword12", HttpStatusCode.Unauthorized,
        "UNAUTHORIZED (401)")]
    public async Task
        Test_Request_Should_Return_Unauthorized_Or_OK_Depending_On_Credential_Validation_When_Auth_Is_Enabled(
            string clientCredentials, HttpStatusCode expectedResponseCode, string expectedResponseBody)
    {
        //arrange
        var path = "/protected/resource";
        var correctBasic = "correctMan124:Correxpassword12";
        var responseBody = "Hello from handler";

        _testServer.AddAuthentication(async c =>
        {
            await Task.Delay(0);
            return AuthUtility.ValidateBasicAuthScheme(c, correctBasic);
        });
        _testServer.AddAction("GET", path,
            async c => { await c.WriteTextToStream(responseBody, HttpStatusCode.OK); });

        //start the server on a different thread 
        if (_counter == 0)
        {
            try
            {
                await TestHelpers.StartTestServer("http://localhost:6082/", _testServer);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown when starting test server");
            }
        }

        _counter++;

        //add auth header
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientCredentials));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);


        //act
        var requestUri = "http://localhost:6082" + path;
        var response = await _httpClient.GetAsync(requestUri);
        var actualResponseBody = await response.Content.ReadAsStringAsync();


        //assert
        Assert.Equal(expectedResponseBody, actualResponseBody);
        Assert.Equal(expectedResponseCode, response.StatusCode);
    }
}