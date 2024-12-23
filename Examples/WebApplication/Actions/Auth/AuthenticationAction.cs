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
        
        //authenticate user

        var header = context.Request.Headers["x-api-key"];
        if (string.IsNullOrEmpty(header))
        {
            return new AuthResponse { IsAuthenticated = false, ClaimsPrincipal = new ClaimsPrincipal() };
        }

        if (!header.Equals("ya mom"))
        {
            return new AuthResponse { IsAuthenticated = false, ClaimsPrincipal = new ClaimsPrincipal() };
        }
        
        
        await Task.Delay(0);

        //return auth state and claims 
        return new AuthResponse { IsAuthenticated = true, ClaimsPrincipal = new ClaimsPrincipal() };
    }
}