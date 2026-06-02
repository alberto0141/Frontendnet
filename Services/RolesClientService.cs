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

        var roles = await response.Content.ReadFromJsonAsync<List<Rol>>(
            cancellationToken
        );

        return roles ?? [];
    }
}