using System.Net;
using frontendnet.Models;
using frontendnet.Security;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = AppRoles.Administrator)]
public class BitacoraController(
    BitacoraClientService bitacora,
    ILogger<BitacoraController> logger
) : Controller
{
    private static class ActionNames
    {
        public const string Logout = "Salir";
    }

    private static class ControllerNames
    {
        public const string Auth = "Auth";
    }

    private static class ErrorMessages
    {
        public const string GenericLoadError = "No ha sido posible cargar la bitácora. Inténtelo nuevamente.";
    }

    [HttpGet]
    public async Task<IActionResult> IndexAsync(CancellationToken cancellationToken)
    {
        try
        {
            var lista = await bitacora.GetAsync(cancellationToken);
            return View(lista);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            return RedirectToAction(ActionNames.Logout, ControllerNames.Auth);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al obtener la bitácora.");
            ModelState.AddModelError(string.Empty, ErrorMessages.GenericLoadError);
            return View(new List<Bitacora>());
        }
    }
}