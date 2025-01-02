using Microsoft.Extensions.Logging;
using Swytch.App;

namespace tests;

public class SwytchAuthenticationTests
{
    private static readonly SwytchApp TestServer = new();
    private static int _counter = 0;
    private readonly ILogger<SwytchAuthenticationTests> _logger;
    private static readonly HttpClient HttpClient = new HttpClient();

    public SwytchAuthenticationTests()
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        _logger = factory.CreateLogger<SwytchAuthenticationTests>();
    }
    
    
    //test auth handlers incorrect
    
    //test auth handler correct
    
}