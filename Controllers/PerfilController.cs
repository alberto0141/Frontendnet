using System.Net;
using System.Security.Claims;
using frontendnet.Models;
using frontendnet.Security;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = AppRoles.AuthenticatedRoles)]
public class PerfilController(
    PerfilClientService perfil,
    ILogger<PerfilController> logger
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
        public const string InvalidSession = "La sesión no contiene información válida.";
        public const string GenericLoadError = "No ha sido posible cargar el perfil. Inténtelo nuevamente.";
    }

    [HttpGet]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> IndexAsync(CancellationToken cancellationToken)
    {
        try
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            var nombre = User.FindFirstValue(ClaimTypes.GivenName);
            var rol = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(rol))
            {
                logger.LogWarning("Sesión inválida: faltan claims básicos del usuario.");
                return RedirectToAction(ActionNames.Logout, ControllerNames.Auth);
            }

            var usuario = new PerfilUsuario
            {
                Email = email,
                Nombre = nombre,
                Rol = rol,
                TiempoRestante = await perfil.ObtenTiempoAsync(cancellationToken)
            };

            return View(usuario);
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
            logger.LogError(ex, "Error al cargar el perfil del usuario {Usuario}.", User.Identity?.Name ?? "Desconocido");
            ModelState.AddModelError(string.Empty, ErrorMessages.GenericLoadError);

            return View(new PerfilUsuario
            {
                Email = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                Nombre = User.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
                Rol = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty,
                TiempoRestante = null
            });
        }
    }
}