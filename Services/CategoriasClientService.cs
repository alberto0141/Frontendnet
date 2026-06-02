using System.Net.Http.Json;
using frontendnet.Models;

namespace frontendnet.Services;

public class CategoriasClientService(HttpClient client)
{
    public async Task<List<Categoria>> GetAsync(CancellationToken cancellationToken = default)
    {
        var paginado = await client.GetFromJsonAsync<ApiPagedResponse<Categoria>>(
            ApiRoutes.Categories,
            cancellationToken
        );

        return paginado?.Data ?? [];
    }

    public async Task<Categoria?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        return await client.GetFromJsonAsync<Categoria>(
            ApiRoutes.CategoryById(id),
            cancellationToken
        );
    }

    public async Task PostAsync(Categoria categoria, CancellationToken cancellationToken = default)
    {
        ValidateCategoria(categoria);

        using var response = await client.PostAsJsonAsync(
            ApiRoutes.Categories,
            new { nombre = categoria.Nombre },
            cancellationToken
        );

        await ApiClientHelper.LanzarSiErrorAsync(response, cancellationToken);
    }

    public async Task PutAsync(Categoria categoria, CancellationToken cancellationToken = default)
    {
        ValidateCategoria(categoria);

        if (categoria.CategoriaId is null)
        {
            throw new ArgumentException("La categoría debe tener un identificador válido.", nameof(categoria));
        }

        ValidateId(categoria.CategoriaId.Value);

        using var response = await client.PutAsJsonAsync(
            ApiRoutes.CategoryById(categoria.CategoriaId.Value),
            new { nombre = categoria.Nombre },
            cancellationToken
        );

        await ApiClientHelper.LanzarSiErrorAsync(response, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        ValidateId(id);

        var response = await client.DeleteAsync(
            ApiRoutes.CategoryById(id),
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "El identificador debe ser mayor a cero.");
        }
    }

    private static void ValidateCategoria(Categoria categoria)
    {
        ArgumentNullException.ThrowIfNull(categoria);

        if (string.IsNullOrWhiteSpace(categoria.Nombre))
        {
            throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(categoria));
        }

        categoria.Nombre = categoria.Nombre.Trim();
    }

}