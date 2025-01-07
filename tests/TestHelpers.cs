using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Swytch.App;
using Swytch.utilities;

namespace tests;

public static class TestHelpers
{
    public async static Task StartTestServer(string uri, SwytchApp testServer)
    {
        await Task.Delay(0);
        _ = Task.Run(async () => { await testServer.Listen(uri); });
    }

    public static string CreateSampleTokenToken(string secrete, int expires,string customeClaim)
    {
        var claims = new List<Claim>
        {
            new Claim("iss", "testProject"),
            new Claim("aud", "testProjectMethod"),
            new Claim("customeClaim", customeClaim),
        };

        var token = AuthUtility.CreateBearerToken(secrete, expires, claims);
        return token;
    }

    public static string GenerateSampleSecreteKey()
    {
        byte[] key = new byte[64]; // 64 bytes = 512 bits
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(key);
        }
        return Convert.ToBase64String(key);
        
    }

    public static TokenValidationParameters GeTokenValidationParameters(bool validIss=true, bool validAud= true,
        bool validLifetime =true)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            // IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = validIss,
            // ValidIssuer = "https://your-issuer.com",
            ValidateAudience = validAud,
            // ValidAudience = "https://your-audience.com",
            ValidateLifetime = validLifetime,
            ClockSkew = TimeSpan.Zero
        };
    }
}