using System.Security.Claims;
using frontendnet.Services;
using Microsoft.AspNetCore.Http;

namespace frontendnet.Middlewares;

public class RefrescaTokenDelegatingHandler(
    AuthClientService auth,
    IHttpContextAccessor httpContextAccessor
) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.Headers.TryGetValues(SecurityHeaderNames.RefreshedAuthorizationHeader, out var values))
        {
            var refreshedJwt = values.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(refreshedJwt))
            {
                var user = httpContextAccessor.HttpContext?.User;

                var email = user?.FindFirstValue(ClaimTypes.Name);
                var nombre = user?.FindFirstValue(ClaimTypes.GivenName);
                var rol = user?.FindFirstValue(ClaimTypes.Role);

                if (!string.IsNullOrWhiteSpace(email) &&
                    !string.IsNullOrWhiteSpace(nombre) &&
                    !string.IsNullOrWhiteSpace(rol))
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, email),
                        new(ClaimTypes.GivenName, nombre),
                        new(SecurityHeaderNames.JwtClaim, refreshedJwt),
                        new(ClaimTypes.Role, rol)
                    };

                    await auth.IniciaSesionAsync(claims);
                }
            }
        }

        response.EnsureSuccessStatusCode();
        return response;
    }
}