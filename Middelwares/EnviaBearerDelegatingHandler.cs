using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace frontendnet.Middlewares;

public class EnviaBearerDelegatingHandler(
    IHttpContextAccessor httpContextAccessor
) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var jwt = httpContextAccessor.HttpContext?.User.FindFirst(SecurityHeaderNames.JwtClaim)?.Value;

        if (!string.IsNullOrWhiteSpace(jwt))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(
                SecurityHeaderNames.AuthorizationScheme,
                jwt
            );
        }

        return base.SendAsync(request, cancellationToken);
    }
}