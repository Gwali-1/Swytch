using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Swytch.Structures;

namespace WebApplication.Actions.Auth;

public class AuthenticationAction
{
    private readonly ILogger<AuthenticationAction> _logger;

    public AuthenticationAction(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AuthenticationAction>();
    }

    public async Task<AuthResponse> AuthenticateUser(RequestContext context)
    {
        _logger.LogInformation("Request to authenticate user");
        
        await Task.Delay(0);
        //authenticate ueser 

        //retuen auth state and claims 
        return new AuthResponse { IsAuthenticated = true, ClaimsPrincipal = new ClaimsPrincipal() };
    }
}