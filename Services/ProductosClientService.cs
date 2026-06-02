using System.Net;
using System.Net.Http.Json;
using frontendnet.Models;

namespace frontendnet.Services;

public class ProductosClientService(HttpClient client)
{
    public async Task<List<Producto>> GetAsync(
        string? search,
        CancellationToken cancellationToken = default
    )
    {
        using var response = await client.GetAsync(
            ApiRoutes.ProductsSearch(search),
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        var productos = await response.Content.ReadFromJsonAsync<ApiPagedResponse<Producto>>(
            cancellationToken: cancellationToken
        );

        return productos?.Data ?? [];
    }

    public async Task<Producto?> GetAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        ValidateId(id, nameof(id));

        using var response = await client.GetAsync(
            ApiRoutes.ProductById(id),
            cancellationToken
        );

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Producto>(
            cancellationToken: cancellationToken
        );
    }

    public async Task PostAsync(
        Producto producto,
        CancellationToken cancellationToken = default
    )
    {
        ValidateProducto(producto);

        using var response = await client.PostAsJsonAsync(
            ApiRoutes.Products,
            producto,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    public async Task PutAsync(
        Producto producto,
        CancellationToken cancellationToken = default
    )
    {
        ValidateProducto(producto);
        ValidateId(producto.ProductoId, nameof(producto.ProductoId));

        using var response = await client.PutAsJsonAsync(
            ApiRoutes.ProductById(producto.ProductoId),
            producto,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        ValidateId(id, nameof(id));

        using var response = await client.DeleteAsync(
            ApiRoutes.ProductById(id),
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    public async Task PostAsync(
        int id,
        int categoriaId,
        CancellationToken cancellationToken = default
    )
    {
        ValidateId(id, nameof(id));
        ValidateId(categoriaId, nameof(categoriaId));

        using var response = await client.PostAsJsonAsync(
            ApiRoutes.ProductCategory(id),
            new { categoriaid = categoriaId },
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(
        int id,
        int categoriaId,
        CancellationToken cancellationToken = default
    )
    {
        ValidateId(id, nameof(id));
        ValidateId(categoriaId, nameof(categoriaId));

        using var response = await client.DeleteAsync(
            ApiRoutes.ProductCategoryById(id, categoriaId),
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "El identificador debe ser mayor a cero."
            );
        }
    }

    private static void ValidateProducto(Producto producto)
    {
        ArgumentNullException.ThrowIfNull(producto);

        if (string.IsNullOrWhiteSpace(producto.Titulo))
        {
            throw new ArgumentException(
                "El título del producto es obligatorio.",
                nameof(producto)
            );
        }

        if (string.IsNullOrWhiteSpace(producto.Descripcion))
        {
            throw new ArgumentException(
                "La descripción del producto es obligatoria.",
                nameof(producto)
            );
        }

        if (producto.Precio <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(producto),
                "El precio del producto debe ser mayor a cero."
            );
        }

        producto.Titulo = producto.Titulo.Trim();
        producto.Descripcion = producto.Descripcion.Trim();
    }
}