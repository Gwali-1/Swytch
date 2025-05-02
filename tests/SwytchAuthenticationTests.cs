using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swytch.App;
using Swytch.Extensions;
using Swytch.Structures;
using Swytch.utilities;
using Xunit;

namespace tests;

public class SwytchAuthenticationTests : IDisposable
{
    private readonly SwytchApp _testServer = new();
    private static int _counter = 0;
    private readonly ILogger<SwytchAuthenticationTests> _logger;
    private readonly HttpClient _httpClient = new();

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


        _testServer.AddAction(new [] {RequestMethod.GET}, path,
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
        await Task.Delay(10);
        var response = await _httpClient.GetAsync(requestUri);
        var actualResponseBody = await response.Content.ReadAsStringAsync();


        //assert
        Assert.Equal(expectedResponse, actualResponseBody);
        Assert.Equal(responseCode, response.StatusCode);
    }
    
    
    
    [Fact]
    public async Task Test_Request_Should_Return_Response_For_Request_To_Static_Server_Regardless_Of_Auth_Status()
    {
        // ARRANGE
        var correctByteLength = 732;
        var requestPath ="/swytchserver/static/gitgit.png"; 
        var responseCode = HttpStatusCode.OK;

        _testServer.AddAuthentication(async c =>
        {
            await Task.Delay(0);
            return AuthUtility.ValidateBasicAuthScheme(c, "name:password");

        });
        _testServer.AddStaticServer();

        //start the server on a different thread 
        try
        {
            await TestHelpers.StartTestServer("http://localhost:6084/", _testServer);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception thrown when starting test server");
        }

        //ACT
        
        var requestUri = "http://localhost:6084" + requestPath;
        await Task.Delay(10);
        var response = await _httpClient.GetAsync(requestUri);
        var bytesStatic = await response.Content.ReadAsByteArrayAsync();


        //ASSERT
        
        Assert.Equal(correctByteLength, bytesStatic.Length);
        Assert.Equal(responseCode, response.StatusCode);
    }


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
        Test_Request_Should_Return_Unauthorized_Or_OK_Depending_On_Credential_Validation_When_Basic_Auth_Is_Enabled(
            string clientCredentials, HttpStatusCode expectedResponseCode, string expectedResponseBody)
    {
        //ARRANGE

        var path = "/protected/resource";
        var correctBasic = "correctMan124:Correxpassword12";
        var responseBody = "Hello from handler";

        _testServer.AddAuthentication(async c =>
        {
            await Task.Delay(0);
            return AuthUtility.ValidateBasicAuthScheme(c, correctBasic);
        });
        _testServer.AddAction(new [] {RequestMethod.GET}, path,
            async c => { await c.WriteTextToStream(responseBody, HttpStatusCode.OK); });


        //start server instance
        try
        {
            await TestHelpers.StartTestServer("http://localhost:6082/", _testServer);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception thrown when starting test server");
        }


        //add auth header
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientCredentials));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);


        //ACT

        var requestUri = "http://localhost:6082" + path;
        await Task.Delay(10);
        var response = await _httpClient.GetAsync(requestUri);
        var actualResponseBody = await response.Content.ReadAsStringAsync();


        //ASSERT

        Assert.Equal(expectedResponseBody, actualResponseBody);
        Assert.Equal(expectedResponseCode, response.StatusCode);
    }


    [Theory]
    [InlineData(true, "Malaman",
        HttpStatusCode.OK, "Malaman")]
    [InlineData(true, "Justice",
        HttpStatusCode.OK, "Justice")]
    [InlineData(true, "Mangossssssss",
        HttpStatusCode.OK, "Mangossssssss")]
    [InlineData(true, "Reader man is here for sure",
        HttpStatusCode.OK, "Reader man is here for sure")]
    [InlineData(true, "Chamber of things",
        HttpStatusCode.OK, "Chamber of things")]
    [InlineData(false, "claim1",
        HttpStatusCode.Unauthorized, "UNAUTHORIZED (401)")]
    [InlineData(false, "claim2",
        HttpStatusCode.Unauthorized, "UNAUTHORIZED (401)")]
    [InlineData(false, "claim3",
        HttpStatusCode.Unauthorized, "UNAUTHORIZED (401)")]
    [InlineData(false, "claim4",
        HttpStatusCode.Unauthorized, "UNAUTHORIZED (401)")]
    public async Task
        Test_Request_Should_Return_Unauthorized_Or_OK_Depending_On_Credential_Validation_When_Bearer_Auth_Is_Enabled(
            bool correctBearer, string customClaim, HttpStatusCode expectedResponseCode, string expectedResponseBody)
    {
        //ARRANGE
        var path = "/protected/resource";
        var validIssuer = "testProject";
        var validAudience = "testProjectMethod";
        var secreteKey = TestHelpers.GenerateSampleSecreteKey();
        var correctToken = TestHelpers.CreateSampleTokenToken(secreteKey, 6300, customClaim);
        var tokenValidationParameters = TestHelpers.GeTokenValidationParameters();


        _testServer.AddAction( new [] {RequestMethod.GET}, path,
            async c =>
            {
                var claim = c.User.Claims.First(c => c.Type.Equals("customeClaim"));
                await c.WriteTextToStream(claim.Value, HttpStatusCode.OK);
            });

        //set up validation parameters
        tokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secreteKey));
        tokenValidationParameters.ValidIssuer = validIssuer;
        tokenValidationParameters.ValidAudience = validAudience;

        //start server instance
        try
        {
            await TestHelpers.StartTestServer("http://localhost:6084/", _testServer);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception thrown when starting test server");
        }


        _testServer.AddAuthentication(async c =>
        {
            await Task.Delay(0);
            return AuthUtility.ValidateTokenAuthScheme(c,  tokenValidationParameters);
        });


        //ACT

        //create auth token
        var token = correctToken;
        if (!correctBearer)
        {
            token = TestHelpers.CreateSampleTokenToken(TestHelpers.GenerateSampleSecreteKey(), 3600, "wrongClaim");
        }

        //add auth header
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var requestUri = "http://localhost:6084" + path;
        await Task.Delay(10);
        var response = await _httpClient.GetAsync(requestUri);
        var actualResponseBody = await response.Content.ReadAsStringAsync();


        //ASSERT

        Assert.Equal(expectedResponseBody, actualResponseBody);
        Assert.Equal(expectedResponseCode, response.StatusCode);
    }

    public void Dispose()
    {
        _testServer.Stop();
        _testServer.Close();
        _httpClient.Dispose();
    }
}