using System.Security.Claims;

namespace Swytch.Structures;

public struct AuthResponse
{
    public bool IsAuthenticated { get; set; }
    public ClaimsPrincipal? ClaimsPrincipal { get; set; }
}

public delegate Task<AuthResponse> AuthHandler(RequestContext context);