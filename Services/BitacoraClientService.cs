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

        var bitacora = await response.Content.ReadFromJsonAsync<List<Bitacora>>(
            cancellationToken
        );

        return bitacora ?? [];
    }
}