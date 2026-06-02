using System.Net;
using frontendnet.Models;
using frontendnet.Security;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = AppRoles.AuthenticatedRoles)]
public class PedidosController(
    PedidosClientService pedidos,
    ILogger<PedidosController> logger
) : Controller
{
    private static class ActionNames
    {
        public const string Logout = "Salir";
        public const string Index = nameof(Index);
        public const string AdminIndex = nameof(AdminIndex);
        public const string Detalle = nameof(Detalle);
    }

    private static class ControllerNames
    {
        public const string Auth = "Auth";
    }

    private static class ErrorMessages
    {
        public const string GenericLoadError = "No ha sido posible cargar los pedidos. Inténtelo nuevamente.";
        public const string PedidoNoDisponible = "El pedido no existe o no tiene permiso para verlo.";
        public const string EstadoInvalido = "El estado indicado no es válido.";
        public const string GenericActionError = "No ha sido posible realizar la acción. Inténtelo nuevamente.";
        public const string InvalidId = "El identificador solicitado no es válido.";
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.User)]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var lista = await pedidos.GetMisPedidosAsync(cancellationToken);
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
            logger.LogError(ex, "Error al obtener mis pedidos.");
            ModelState.AddModelError(string.Empty, ErrorMessages.GenericLoadError);
            return View(new List<Pedido>());
        }
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.AuthenticatedRoles)]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(ErrorMessages.InvalidId);
        }

        try
        {
            var pedido = await pedidos.GetByIdAsync(id, cancellationToken);

            if (pedido is null)
            {
                TempData["Error"] = ErrorMessages.PedidoNoDisponible;
                return RedirectSegunRol();
            }

            return View(pedido);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            return RedirectToAction(ActionNames.Logout, ControllerNames.Auth);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            TempData["Error"] = ErrorMessages.PedidoNoDisponible;
            return RedirectSegunRol();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al obtener pedido {PedidoId}.", id);
            TempData["Error"] = ErrorMessages.GenericLoadError;
            return RedirectSegunRol();
        }
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<IActionResult> AdminIndex(CancellationToken cancellationToken)
    {
        try
        {
            var lista = await pedidos.GetTodosAsync(cancellationToken);
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
            logger.LogError(ex, "Error al obtener todos los pedidos.");
            ModelState.AddModelError(string.Empty, ErrorMessages.GenericLoadError);
            return View(new List<Pedido>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<IActionResult> CambiarEstado(
        int id,
        string estado,
        CancellationToken cancellationToken
    )
    {
        if (id <= 0)
        {
            return BadRequest(ErrorMessages.InvalidId);
        }

        if (!PedidoEstados.EsValido(estado))
        {
            TempData["Error"] = ErrorMessages.EstadoInvalido;
            return RedirectToAction(ActionNames.Detalle, new { id });
        }

        try
        {
            await pedidos.CambiarEstadoAsync(id, estado, cancellationToken);
            TempData["Exito"] = "Estado actualizado correctamente.";
            return RedirectToAction(ActionNames.Detalle, new { id });
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
            logger.LogError(ex, "Error al cambiar estado del pedido {PedidoId}.", id);
            TempData["Error"] = ErrorMessages.GenericActionError;
            return RedirectToAction(ActionNames.Detalle, new { id });
        }
    }

    private IActionResult RedirectSegunRol() =>
        User.IsInRole(AppRoles.Administrator)
            ? RedirectToAction(ActionNames.AdminIndex)
            : RedirectToAction(ActionNames.Index);
}
