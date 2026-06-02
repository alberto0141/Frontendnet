using System.ComponentModel.DataAnnotations;
using System.Net;
using frontendnet.Models;
using frontendnet.Security;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace frontendnet;

[Authorize(Roles = AppRoles.Administrator)]
public class UsuariosController(
    UsuariosClientService usuarios,
    RolesClientService roles,
    ILogger<UsuariosController> logger
) : Controller
{
    private static readonly EmailAddressAttribute EmailValidator = new();

    private static class ActionNames
    {
        public const string Logout = "Salir";
    }

    private static class ControllerNames
    {
        public const string Auth = "Auth";
    }

    private static class ViewDataKeys
    {
        public const string ErrorMessage = "ErrorMessage";
        public const string CanEdit = "PuedeEditar";
    }

    private static class ErrorMessages
    {
        public const string GenericActionError = "No ha sido posible realizar la acción. Inténtelo nuevamente.";
        public const string GenericLoadError = "No ha sido posible cargar la información. Inténtelo nuevamente.";
        public const string InvalidEmail = "El correo electrónico solicitado no es válido.";
        public const string InvalidUser = "Los datos del usuario no son válidos.";
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var lista = await usuarios.GetAsync(cancellationToken);
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
            logger.LogError(ex, "Error al obtener la lista de usuarios.");
            ModelState.AddModelError(string.Empty, ErrorMessages.GenericLoadError);
            return View(new List<Usuario>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(
        string id,
        CancellationToken cancellationToken
    )
    {
        var email = NormalizeEmailOrNull(id);

        if (email is null)
        {
            return BadRequest(ErrorMessages.InvalidEmail);
        }

        try
        {
            var item = await usuarios.GetAsync(email, cancellationToken);

            if (item is null)
            {
                return NotFound();
            }

            return View(item);
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
            logger.LogError(ex, "Error al consultar el usuario {Email}.", email);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Crear(CancellationToken cancellationToken)
    {
        try
        {
            await RolesDropDownListAsync(null, cancellationToken);
            return View();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al cargar la vista de creación de usuario.");
            ModelState.AddModelError(string.Empty, ErrorMessages.GenericLoadError);
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        UsuarioPwd itemToCreate,
        CancellationToken cancellationToken
    )
    {
        if (itemToCreate is null)
        {
            return BadRequest(ErrorMessages.InvalidUser);
        }

        NormalizeUsuarioPwd(itemToCreate);

        if (!ModelState.IsValid)
        {
            await RolesDropDownListAsync(itemToCreate.Rol, cancellationToken);
            return View(itemToCreate);
        }

        try
        {
            await usuarios.PostAsync(itemToCreate, cancellationToken);
            return RedirectToAction(nameof(Index));
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
            logger.LogError(ex, "Error al crear el usuario {Email}.", itemToCreate.Email);
            ModelState.AddModelError(nameof(UsuarioPwd.Email), ErrorMessages.GenericActionError);
            await RolesDropDownListAsync(itemToCreate.Rol, cancellationToken);
            return View(itemToCreate);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(
        string id,
        CancellationToken cancellationToken
    )
    {
        var email = NormalizeEmailOrNull(id);

        if (email is null)
        {
            return BadRequest(ErrorMessages.InvalidEmail);
        }

        try
        {
            var itemToEdit = await usuarios.GetAsync(email, cancellationToken);

            if (itemToEdit is null)
            {
                return NotFound();
            }

            await RolesDropDownListAsync(itemToEdit.Rol, cancellationToken);
            ViewBag.PuedeEditar = User.Identity?.Name != itemToEdit.Email;

            return View(itemToEdit);
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
            logger.LogError(ex, "Error al cargar el usuario {Email} para edición.", email);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        string id,
        Usuario itemToEdit,
        CancellationToken cancellationToken
    )
    {
        var routeEmail = NormalizeEmailOrNull(id);

        if (routeEmail is null || itemToEdit is null)
        {
            return BadRequest(ErrorMessages.InvalidUser);
        }

        NormalizeUsuario(itemToEdit);

        if (!string.Equals(routeEmail, itemToEdit.Email, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(ErrorMessages.InvalidEmail);
        }

        if (!ModelState.IsValid)
        {
            await RolesDropDownListAsync(itemToEdit.Rol, cancellationToken);
            ViewBag.PuedeEditar = User.Identity?.Name != itemToEdit.Email;
            return View(itemToEdit);
        }

        try
        {
            await usuarios.PutAsync(itemToEdit, cancellationToken);
            return RedirectToAction(nameof(Index));
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
            logger.LogError(ex, "Error al editar el usuario {Email}.", itemToEdit.Email);
            ModelState.AddModelError(nameof(Usuario.Email), ErrorMessages.GenericActionError);
            await RolesDropDownListAsync(itemToEdit.Rol, cancellationToken);
            ViewBag.PuedeEditar = User.Identity?.Name != itemToEdit.Email;
            return View(itemToEdit);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(
        string id,
        bool? showError = false,
        CancellationToken cancellationToken = default
    )
    {
        var email = NormalizeEmailOrNull(id);

        if (email is null)
        {
            return BadRequest(ErrorMessages.InvalidEmail);
        }

        try
        {
            var itemToDelete = await usuarios.GetAsync(email, cancellationToken);

            if (itemToDelete is null)
            {
                return NotFound();
            }

            if (showError.GetValueOrDefault())
            {
                ViewData[ViewDataKeys.ErrorMessage] = ErrorMessages.GenericActionError;
            }

            ViewBag.PuedeEditar = User.Identity?.Name != itemToDelete.Email;

            return View(itemToDelete);
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
            logger.LogError(ex, "Error al cargar el usuario {Email} para eliminación.", email);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(
        string id,
        CancellationToken cancellationToken
    )
    {
        var email = NormalizeEmailOrNull(id);

        if (email is null)
        {
            return BadRequest(ErrorMessages.InvalidEmail);
        }

        if (string.Equals(User.Identity?.Name, email, StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("El usuario {Email} intentó eliminar su propia cuenta desde administración.", email);
            return RedirectToAction(nameof(Eliminar), new { id = email, showError = true });
        }

        try
        {
            await usuarios.DeleteAsync(email, cancellationToken);
            return RedirectToAction(nameof(Index));
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
            logger.LogError(ex, "Error al eliminar el usuario {Email}.", email);
            return RedirectToAction(nameof(Eliminar), new { id = email, showError = true });
        }
    }

    private static string? NormalizeEmailOrNull(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        var normalizedEmail = email.Trim().ToLowerInvariant();

        return EmailValidator.IsValid(normalizedEmail)
            ? normalizedEmail
            : null;
    }

    private static void NormalizeUsuario(Usuario usuario)
    {
        usuario.Email = NormalizeEmailOrNull(usuario.Email) ?? string.Empty;
        usuario.Nombre = usuario.Nombre.Trim();
        usuario.Rol = usuario.Rol.Trim();
    }

    private static void NormalizeUsuarioPwd(UsuarioPwd usuario)
    {
        usuario.Email = NormalizeEmailOrNull(usuario.Email) ?? string.Empty;
        usuario.Nombre = usuario.Nombre.Trim();
        usuario.Rol = usuario.Rol.Trim();
    }

    private async Task RolesDropDownListAsync(
        object? rolSeleccionado,
        CancellationToken cancellationToken
    )
    {
        var listado = await roles.GetAsync(cancellationToken);
        ViewBag.Rol = new SelectList(listado, "Nombre", "Nombre", rolSeleccionado);
    }
}