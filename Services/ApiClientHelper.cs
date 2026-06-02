using System.Net;
using System.Net.Http.Json;
using frontendnet.Models;

namespace frontendnet.Services;

internal static class ApiClientHelper
{
    internal static async Task LanzarSiErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken
    )
    {
        if (response.IsSuccessStatusCode) return;

        // 401/403: lanzar HttpRequestException para que los catch de controladores existentes funcionen
        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            throw new HttpRequestException(
                response.StatusCode.ToString(),
                null,
                response.StatusCode
            );
        }

        ApiErrorResponse? errorResponse = null;
        try
        {
            errorResponse = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(
                cancellationToken: cancellationToken
            );
        }
        catch { /* body no es JSON válido */ }

        throw new ApiClientException(response.StatusCode, errorResponse);
    }
}
