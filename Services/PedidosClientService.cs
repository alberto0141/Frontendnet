using System.Net;
using System.Net.Http.Json;
using frontendnet.Models;

namespace frontendnet.Services;

public class PedidosClientService(HttpClient client)
{
    public async Task<Pedido?> CrearAsync(
        ResumenCarrito carrito,
        CancellationToken cancellationToken = default
    )
    {
        var request = new CrearPedidoRequest
        {
            Items = carrito.Items
                .Select(i => new CrearPedidoItemRequest
                {
                    ProductoId = i.ProductoId,
                    Cantidad = i.Cantidad
                })
                .ToList()
        };

        using var response = await client.PostAsJsonAsync(
            ApiRoutes.Pedidos,
            request,
            cancellationToken
        );

        LanzarSiError(response.StatusCode);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Pedido>(cancellationToken);
    }

    public async Task<List<Pedido>> GetMisPedidosAsync(
        CancellationToken cancellationToken = default
    )
    {
        using var response = await client.GetAsync(
            ApiRoutes.MisPedidos,
            cancellationToken
        );

        LanzarSiError(response.StatusCode);
        response.EnsureSuccessStatusCode();

        var paginado = await response.Content.ReadFromJsonAsync<ApiPagedResponse<Pedido>>(cancellationToken);

        return paginado?.Data ?? [];
    }

    public async Task<Pedido?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        using var response = await client.GetAsync(
            ApiRoutes.PedidoById(id),
            cancellationToken
        );

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        LanzarSiError(response.StatusCode);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Pedido>(cancellationToken);
    }

    public async Task<List<Pedido>> GetTodosAsync(
        CancellationToken cancellationToken = default
    )
    {
        using var response = await client.GetAsync(
            ApiRoutes.Pedidos,
            cancellationToken
        );

        LanzarSiError(response.StatusCode);
        response.EnsureSuccessStatusCode();

        var paginado = await response.Content.ReadFromJsonAsync<ApiPagedResponse<Pedido>>(cancellationToken);

        return paginado?.Data ?? [];
    }

    public async Task CambiarEstadoAsync(
        int id,
        string estado,
        CancellationToken cancellationToken = default
    )
    {
        var request = new CambiarEstadoPedidoRequest { Estado = estado };

        using var response = await client.PatchAsJsonAsync(
            ApiRoutes.PedidoEstadoById(id),
            request,
            cancellationToken
        );

        LanzarSiError(response.StatusCode);
        response.EnsureSuccessStatusCode();
    }

    private static void LanzarSiError(HttpStatusCode statusCode)
    {
        if (statusCode == HttpStatusCode.Unauthorized)
        {
            throw new HttpRequestException(
                "La sesión ha expirado.",
                null,
                HttpStatusCode.Unauthorized
            );
        }

        if (statusCode == HttpStatusCode.Forbidden)
        {
            throw new HttpRequestException(
                "No tiene permisos para realizar esta acción.",
                null,
                HttpStatusCode.Forbidden
            );
        }

        if (statusCode == HttpStatusCode.BadRequest)
        {
            throw new HttpRequestException(
                "Los datos enviados no son válidos.",
                null,
                HttpStatusCode.BadRequest
            );
        }
    }
}
