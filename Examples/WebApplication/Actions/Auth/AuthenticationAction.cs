using System.Security.Claims;
using Swytch.Structures;

namespace WebApplication.Actions.Auth;

public class AuthenticationAction
{
    public async Task<AuthResponse> AuthenticateUser(RequestContext context)
    {
        await Task.Delay(0);
        //authenticate ueser 

        //retuen auth state and claims 
        return new AuthResponse { IsAuthenticated = true, ClaimsPrincipal = new ClaimsPrincipal() };
    }
}