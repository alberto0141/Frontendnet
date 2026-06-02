using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using frontendnet.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace frontendnet.Services;

public class AuthClientService(
    HttpClient client,
    IHttpContextAccessor httpContextAccessor
)
{
    private static readonly TimeSpan SessionDuration = TimeSpan.FromMinutes(30);

    public async Task<AuthUser> ObtenerTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        var usuario = new Login
        {
            Email = email.Trim().ToLowerInvariant(),
            Password = password
        };

        var response = await client.PostAsJsonAsync(
            ApiRoutes.AuthLogin,
            usuario,
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new UnauthorizedAccessException("Credenciales no válidas.");
        }

        var token = await response.Content.ReadFromJsonAsync<AuthUser>(
            cancellationToken
        );

        if (token is null || string.IsNullOrWhiteSpace(token.Jwt))
        {
            throw new InvalidOperationException("La respuesta de autenticación no es válida.");
        }

        return token;
    }

    public async Task RegistrarAsync(
        RegistroPublico model,
        CancellationToken cancellationToken = default
    )
    {
        var payload = new
        {
            email = model.Email.Trim().ToLowerInvariant(),
            nombre = model.Nombre.Trim(),
            password = model.Password
        };

        using var response = await client.PostAsJsonAsync(
            ApiRoutes.AuthRegistro,
            payload,
            cancellationToken
        );

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new HttpRequestException(
                "El correo electrónico ya está registrado.",
                inner: null,
                statusCode: HttpStatusCode.Conflict
            );
        }

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            throw new HttpRequestException(
                "Demasiados intentos. Inténtelo de nuevo más tarde.",
                inner: null,
                statusCode: HttpStatusCode.TooManyRequests
            );
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new HttpRequestException(
                "Los datos ingresados no son válidos.",
                inner: null,
                statusCode: HttpStatusCode.BadRequest
            );
        }

        response.EnsureSuccessStatusCode();
    }

    public async Task IniciaSesionAsync(
        IEnumerable<Claim> claims,
        CancellationToken cancellationToken = default
    )
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext is null)
        {
            throw new InvalidOperationException("No existe un contexto HTTP disponible.");
        }

        var claimsList = claims
            .Where(claim => !string.IsNullOrWhiteSpace(claim.Value))
            .ToList();

        if (claimsList.Count == 0)
        {
            throw new InvalidOperationException("No existen claims válidos para iniciar sesión.");
        }

        var claimsIdentity = new ClaimsIdentity(
            claimsList,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false,
            AllowRefresh = true,
            IssuedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = DateTimeOffset.UtcNow.Add(SessionDuration)
        };

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );
    }
}