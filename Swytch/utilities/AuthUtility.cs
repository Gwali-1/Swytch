using System.Security.Claims;
using System.Text;
using Swytch.Structures;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Swytch.utilities;

public static class AuthUtility
{
    
    /// <summary>
    /// A ready to use basic authentication scheme method/action.This method allows you easily authenticate a basic credential submitted from
    /// a client as basic authentication credentials. It takes in the current request context and the valid basic credentials to perform the authentication
    /// against in the formal "userID:password"
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="credentials">Valid auth credentials in format (userID:password)</param>
    /// <returns></returns>
    public static AuthResponse ValidateBasicAuthScheme(RequestContext context, string credentials)
    {
        string? authHeaderValue = context.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeaderValue))
        {
            return new AuthResponse { IsAuthenticated = false, ClaimsPrincipal = new ClaimsPrincipal() };
        }

        string[] authParts = authHeaderValue.Split(" ");
        if (authParts.Length < 2)
        {
            return new AuthResponse { IsAuthenticated = false, ClaimsPrincipal = new ClaimsPrincipal() };
        }

        byte[] base64CredentialsBytes = Convert.FromBase64String(authParts[1]);
        string[] basic = Encoding.UTF8.GetString(base64CredentialsBytes).Split(":");
        
        if (basic.Length < 2)
        {
            return new AuthResponse { IsAuthenticated = false, ClaimsPrincipal = new ClaimsPrincipal() };
        }
        
        string username = basic[0];
        string password = basic[1];

        string[] validCredentials = credentials.Split(":");
        if (validCredentials.Length < 2)
        {
            return new AuthResponse { IsAuthenticated = false, ClaimsPrincipal = new ClaimsPrincipal() };
        }
        
        if (validCredentials[0] == username && validCredentials[1] == password)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };
            var claimsIdentity = new ClaimsIdentity(claims,authParts[0]);
            return new AuthResponse { IsAuthenticated = true, ClaimsPrincipal = new ClaimsPrincipal(claimsIdentity) };
        }
        return new AuthResponse { IsAuthenticated = false, ClaimsPrincipal = new ClaimsPrincipal() };
    }
    
    
   
    /// <summary>
    /// A ready to use bearer authentication scheme method/action. The method accepts the current  request context, the token to validate
    /// and an instance of TokenValidationParameters type which contains directives on how the token should be validated. eg whether the audience and
    /// issuer of token should be validated, allow any clock descrepancies
    /// </summary>
    /// <param name="context">The current request context</param>
    /// <param name="token">The token to validate</param>
    /// <param name="tokenValidationParameters">An instance of TokenValidationParameters </param>
    /// <returns></returns>
    public static AuthResponse ValidateTokenAuthScheme(RequestContext context, TokenValidationParameters tokenValidationParameters )
    {
        
        string? authHeaderValue = context.Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeaderValue))
        {
            return new AuthResponse { IsAuthenticated = false, ClaimsPrincipal = new ClaimsPrincipal() };
        }

        string[] authParts = authHeaderValue.Split(" ");
        if (authParts.Length < 2)
        {
            return new AuthResponse { IsAuthenticated = false, ClaimsPrincipal = new ClaimsPrincipal() };
        }

        var clientToken = authParts[1];


        JsonWebTokenHandler jsonWebTokenHandler = new JsonWebTokenHandler();
        var tokenValidationResult = jsonWebTokenHandler.ValidateToken(clientToken, tokenValidationParameters);
        if(!tokenValidationResult.IsValid)
        {
            return new AuthResponse { IsAuthenticated = false, ClaimsPrincipal = new ClaimsPrincipal() };
        }

        return new AuthResponse
            { IsAuthenticated = true, ClaimsPrincipal = new ClaimsPrincipal(tokenValidationResult.ClaimsIdentity) };
    }
    
    

    
    /// <summary>
    /// This is a convenience method to allow you to easily create and issue a token by eliminating some boilerplate. The method accepts
    /// your secrete key for the token signature, when the token expires in seconds and a list of claims to be associated with this token
    /// </summary>
    /// <param name="secretKey">Secrete key</param>
    /// <param name="lifeExpireSeconds">Token expiration in seconds</param>
    /// <param name="claims"> a list of claims to be associated with token</param>
    /// <returns></returns>
    public static string CreateBearerToken(string secretKey, int lifeExpireSeconds, List<Claim> claims)
    {
        var key = Encoding.UTF8.GetBytes(secretKey);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = claims.Find(c => c.Type == "iss")?.Value,
            Audience = claims.Find(c => c.Type == "aud")?.Value,
            Expires = DateTime.UtcNow.AddSeconds(lifeExpireSeconds),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        JsonWebTokenHandler jsonWebTokenHandler = new JsonWebTokenHandler();
        var token = jsonWebTokenHandler.CreateToken(descriptor);
        return token;

    }
}