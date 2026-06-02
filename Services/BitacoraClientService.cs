using System.Net.Http.Json;
using frontendnet.Models;

namespace frontendnet.Services;

public class BitacoraClientService(HttpClient client)
{
    public async Task<List<Bitacora>> GetAsync(
        CancellationToken cancellationToken = default
    )
    {
        using var response = await client.GetAsync(
            ApiRoutes.AuditLogs,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        var paginado = await response.Content.ReadFromJsonAsync<ApiPagedResponse<Bitacora>>(
            cancellationToken
        );

        return paginado?.Data ?? [];
    }
}