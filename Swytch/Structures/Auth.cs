namespace Swytch.Structures;

    public struct AuthResponse
    {
        public bool IsAuthenticated;
        public System.Security.Principal.IPrincipal? ClaimsPrincipal;
    }
    
    public delegate Task<AuthResponse> AuthHandler (RequestContext context);