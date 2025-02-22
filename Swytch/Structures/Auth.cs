using System.Security.Claims;

namespace Swytch.Structures;


/// <summary>
/// Response type for an authentication handler , containing a boolean indicator of whether user was authenticated successfully  and a claims principal object
/// with created claims.
/// </summary>

public struct AuthResponse
{
    public bool IsAuthenticated { get; set; }
    public ClaimsPrincipal? ClaimsPrincipal { get; set; }
}


/// <summary>
/// Method signature for an authentication handler. Implementations of methods to authenticate client request must satisfy this delegate.
/// </summary>
public delegate Task<AuthResponse> AuthHandler(RequestContext context);