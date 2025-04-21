
Swytch provides a small but useful set of authentication utility methods to help you get started with simple auth flows.
These helpers are designed to assist in setting up basic authentication and JWT bearer token validation easily and
quickly.

The utility methods live under a static class called `AuthUtility`. They serve as convenience methods to help you
prototype or implement simple protection around your Swytch application quickly.

### Basic Authentication

#### ValidateBasicAuthScheme

This method offers a ready-to-use Basic Authentication check. It allows you to easily authenticate requests using
_base64-encoded credentials_ typically sent via the Authorization header in the Basic scheme.

It takes 2 parameters

**_context_** - The current `RequestContext`, from which headers and request metadata can be accessed.

**_credentials_** - A string in the format `username:password` representing the valid credentials to compare against.

##### Example

```csharp
swytchApp.AddAuthentication(async (context) =>
{    
    var authResult = AuthUtility.ValidateBasicAuthScheme(context, "admin:secret");
    if (authResult.IsAuthenticated)
    {
    //optionally add more claims
     authResult.claimsPrincipal.Claims.Append(new Claim(ClaimTypes.Age, 45));
     return authResult;
    }
    
    return authResult;
});

```

In the example above, we use the built-in `AuthUtility.ValidateBasicAuthScheme` method to perform basic authentication
using a hardcoded credential `admin:secret`.

When a request includes basic authentication headers, the `ValidateBasicAuthScheme` method automatically decodes the
credentials and checks them against the expected string provided (`admin:secret`).

If the credentials match, the method returns an authenticated `AuthResponse` that includes a ClaimsPrincipal with a
single
default claim, the `ClaimTypes.Name`, which is set to the username part of the basic auth string.

You can customize this further by appending additional
claims to the `ClaimsPrincipal` object before returning the response, allowing for extended identity or authorization handling
within your application.

### Bearer Authentication (JWT)

#### CreateBearerToken

This helper method makes it easy to create a JWT bearer token without the boilerplate. You provide a secret key, how
long the
token should live (in seconds), and a list of claims to embed in the token payload.

The method takes 3 parameters

**_secretKey_** - A string used to sign the JWT.

**_lifeExpireSeconds_** - How long the token should be valid, from now.

**_claims_** - A list of System.Security.Claims.Claim objects to attach to the token.

##### Example

```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, "user123"),
    new Claim(ClaimTypes.Role, "admin")
};

var token = AuthUtility.CreateBearerToken("my-super-secret", 3600, claims);

```

#### ValidateBearerToken

This method is a convenience implementation for validating a JWT Bearer Token. It parses and validates a JWT string (
token) using the rules specified in the provided _TokenValidationParameters_.

The method takes in 3 parameters.

_**context**_ - The current RequestContext.

_**token**_ - The raw JWT string from the client, usually from the _Authorization: Bearer token_ header.

_**tokenValidationParameters**_ - An instance of TokenValidationParameters, where you define how the token should be
validated (issuer, audience, expiry, clock skew, etc.).

##### Example

```csharp


swytchApp.AddAuthentication(async (context) =>
{  
var validationParams = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = "swytch-api",
    ValidateAudience = true,
    ValidAudience = "swytch-clients",
    ValidateLifetime = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my-super-secret")),
};  
var authResult = AuthUtility.ValidateBearerToken(context, 'token', validationParams);
    if (authResult.IsAuthenticated)
    {
    //add more claims
     authResult.claimsPrincipal.Claims.Append(new Claim(ClaimTypes.Age, 45));
     return authResult;
    }
    
    return authResult;
});


```

This authentication setup leverages the `ValidateBearerToken` utility to inspect and validate a JWT included in the
request . The `TokenValidationParameters` object specifies how the token
should be validated, such as the expected issuer, audience, lifetime, and signing key.

If the token passes all checks, the utility returns an authenticated `AuthResponse`, which includes a `ClaimsPrincipal`
populated with the claims that were originally embedded in the token when it was created.

You can also append additional claims programmatically before
returning the response, allowing for richer identity and authorization data within your application.




