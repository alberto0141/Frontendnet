using System.Net.Http.Json;
using frontendnet.Models;

namespace frontendnet.Services;

public class RolesClientService(HttpClient client)
{
    public async Task<List<Rol>> GetAsync(
        CancellationToken cancellationToken = default
    )
    {
        using var response = await client.GetAsync(
            ApiRoutes.Roles,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        var paginado = await response.Content.ReadFromJsonAsync<ApiPagedResponse<Rol>>(
            cancellationToken
        );

        return paginado?.Data ?? [];
    }
}