using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using frontendnet.Models;

namespace frontendnet.Services;

public class UsuariosClientService(HttpClient client)
{
    private static readonly EmailAddressAttribute EmailValidator = new();

    public async Task<List<Usuario>> GetAsync(
        CancellationToken cancellationToken = default
    )
    {
        var paginado = await client.GetFromJsonAsync<ApiPagedResponse<Usuario>>(
            ApiRoutes.Users,
            cancellationToken
        );

        return paginado?.Data ?? [];
    }

    public async Task<Usuario?> GetAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedEmail = NormalizeEmail(email);

        return await client.GetFromJsonAsync<Usuario>(
            ApiRoutes.UserByEmail(normalizedEmail),
            cancellationToken
        );
    }

    public async Task PostAsync(
        UsuarioPwd usuario,
        CancellationToken cancellationToken = default
    )
    {
        ValidateUsuarioPwd(usuario);

        var response = await client.PostAsJsonAsync(
            ApiRoutes.Users,
            usuario,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    public async Task PutAsync(
        Usuario usuario,
        CancellationToken cancellationToken = default
    )
    {
        ValidateUsuario(usuario);

        using var response = await client.PutAsJsonAsync(
            ApiRoutes.UserByEmail(usuario.Email),
            new { nombre = usuario.Nombre, rol = usuario.Rol },
            cancellationToken
        );

        await ApiClientHelper.LanzarSiErrorAsync(response, cancellationToken);
    }

    public async Task DeleteAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedEmail = NormalizeEmail(email);

        var response = await client.DeleteAsync(
            ApiRoutes.UserByEmail(normalizedEmail),
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("El correo electrónico es obligatorio.", nameof(email));
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();

        if (!EmailValidator.IsValid(normalizedEmail))
        {
            throw new ArgumentException("El correo electrónico no es válido.", nameof(email));
        }

        return normalizedEmail;
    }

    private static void ValidateUsuario(Usuario usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        usuario.Email = NormalizeEmail(usuario.Email);

        if (string.IsNullOrWhiteSpace(usuario.Nombre))
        {
            throw new ArgumentException("El nombre del usuario es obligatorio.", nameof(usuario));
        }

        if (string.IsNullOrWhiteSpace(usuario.Rol))
        {
            throw new ArgumentException("El rol del usuario es obligatorio.", nameof(usuario));
        }

        usuario.Nombre = usuario.Nombre.Trim();
        usuario.Rol = usuario.Rol.Trim();
    }

    private static void ValidateUsuarioPwd(UsuarioPwd usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        usuario.Email = NormalizeEmail(usuario.Email);

        if (string.IsNullOrWhiteSpace(usuario.Password))
        {
            throw new ArgumentException("La contraseña es obligatoria.", nameof(usuario));
        }

        if (string.IsNullOrWhiteSpace(usuario.Nombre))
        {
            throw new ArgumentException("El nombre del usuario es obligatorio.", nameof(usuario));
        }

        if (string.IsNullOrWhiteSpace(usuario.Rol))
        {
            throw new ArgumentException("El rol del usuario es obligatorio.", nameof(usuario));
        }

        usuario.Nombre = usuario.Nombre.Trim();
        usuario.Rol = usuario.Rol.Trim();
    }
}