namespace frontendnet.Services;

public class PerfilClientService(HttpClient client)
{
    public async Task<string> ObtenTiempoAsync(
        CancellationToken cancellationToken = default
    )
    {
        using var response = await client.GetAsync(
            ApiRoutes.AuthTime,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        var tiempo = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(tiempo))
        {
            throw new InvalidOperationException("La respuesta del tiempo de sesión no es válida.");
        }

        return tiempo;
    }
}