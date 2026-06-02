using System.Net;
using frontendnet.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace frontendnet;

internal static class ApiErrorHelper
{
    internal static void AgregarErrores(
        ModelStateDictionary modelState,
        ApiClientException ex,
        string key,
        string fallback
    )
    {
        // Para errores 5xx: no exponer detalles del backend al usuario
        if ((int)ex.StatusCode >= 500)
        {
            modelState.AddModelError(key, fallback);
            return;
        }

        if (ex.ErrorResponse?.Details?.Count > 0)
        {
            foreach (var detail in ex.ErrorResponse.Details.Where(d => !string.IsNullOrWhiteSpace(d.Msg)))
            {
                modelState.AddModelError(key, detail.Msg!);
            }
            return;
        }

        var mensaje = !string.IsNullOrWhiteSpace(ex.ErrorResponse?.Mensaje)
            ? ex.ErrorResponse.Mensaje
            : fallback;

        modelState.AddModelError(key, mensaje);
    }
}
