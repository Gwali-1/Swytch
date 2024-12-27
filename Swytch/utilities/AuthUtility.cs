using System.Security.Claims;
using Swytch.Structures;

namespace Swytch.utilities;
using System.IdentityModel.Tokens.Jwt;

public static class AuthUtility
{
    public static AuthResponse BasicAuthScheme(RequestContext context, string credentials)
    {
        //get authorization header

        string? authHeaderValue = context.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeaderValue))
        {
            return new AuthResponse { IsAuthenticated = false, ClaimsPrincipal = new ClaimsPrincipal() };
        }

        byte[] basicCredentialsBytes = Convert.FromBase64String(authHeaderValue);
        string[] basic = BitConverter.ToString(basicCredentialsBytes).Split(":");
        string username = basic[0];
        string password = basic[1];

        string[] validCredentials = credentials.Split(":");
        if (validCredentials[0] == username && validCredentials[1] == password)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };
            var claimsIdentity = new ClaimsIdentity(claims);
            return new AuthResponse { IsAuthenticated = true, ClaimsPrincipal = new ClaimsPrincipal(claimsIdentity) };
        }
        return new AuthResponse { IsAuthenticated = false, ClaimsPrincipal = new ClaimsPrincipal() };
    }


    public static AuthResponse TokenAuthSheme(RequestContext context)
    {
        //implementation
        return new AuthResponse();
    }
}