using System.Net;
using frontendnet.Models;
using frontendnet.Security;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = AppRoles.Administrator)]
public class CategoriasController(
    CategoriasClientService categorias,
    ILogger<CategoriasController> logger
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

    private static class ViewDataKeys
    {
        public const string ErrorMessage = "ErrorMessage";
    }

    private static class ErrorMessages
    {
        public const string GenericActionError = "No ha sido posible realizar la acción. Inténtelo nuevamente.";
        public const string GenericLoadError = "No ha sido posible cargar la información. Inténtelo nuevamente.";
        public const string InvalidId = "El identificador solicitado no es válido.";
        public const string ProtectedCategory = "La categoría está protegida y no puede eliminarse.";
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var lista = await categorias.GetAsync(cancellationToken);
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
            logger.LogError(ex, "Error al obtener la lista de categorías.");
            ModelState.AddModelError(string.Empty, ErrorMessages.GenericLoadError);
            return View(new List<Categoria>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(
        int id,
        CancellationToken cancellationToken
    )
    {
        if (!IsValidId(id))
        {
            return BadRequest(ErrorMessages.InvalidId);
        }

        try
        {
            var item = await categorias.GetAsync(id, cancellationToken);

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
            logger.LogError(ex, "Error al consultar la categoría con id {CategoriaId}.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearAsync(
        Categoria itemToCreate,
        CancellationToken cancellationToken
    )
    {
        if (itemToCreate is null)
        {
            return BadRequest();
        }

        NormalizeCategoria(itemToCreate);

        if (!ModelState.IsValid)
        {
            return View(itemToCreate);
        }

        try
        {
            await categorias.PostAsync(itemToCreate, cancellationToken);
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
            logger.LogError(ex, "Error al crear una categoría.");
            ModelState.AddModelError(nameof(Categoria.Nombre), ErrorMessages.GenericActionError);
            return View(itemToCreate);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditarAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        if (!IsValidId(id))
        {
            return BadRequest(ErrorMessages.InvalidId);
        }

        try
        {
            var itemToEdit = await categorias.GetAsync(id, cancellationToken);

            if (itemToEdit is null)
            {
                return NotFound();
            }

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
            logger.LogError(ex, "Error al cargar la categoría con id {CategoriaId} para edición.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarAsync(
        int id,
        Categoria itemToEdit,
        CancellationToken cancellationToken
    )
    {
        if (!IsValidId(id))
        {
            return BadRequest(ErrorMessages.InvalidId);
        }

        if (itemToEdit is null)
        {
            return BadRequest();
        }

        if (itemToEdit.CategoriaId is null || id != itemToEdit.CategoriaId.Value)
        {
            return BadRequest(ErrorMessages.InvalidId);
        }

        NormalizeCategoria(itemToEdit);

        if (!ModelState.IsValid)
        {
            return View(itemToEdit);
        }

        try
        {
            await categorias.PutAsync(itemToEdit, cancellationToken);
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
            logger.LogError(ex, "Error al editar la categoría con id {CategoriaId}.", id);
            ModelState.AddModelError(nameof(Categoria.Nombre), ErrorMessages.GenericActionError);
            return View(itemToEdit);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(
        int id,
        bool? showError = false,
        CancellationToken cancellationToken = default
    )
    {
        if (!IsValidId(id))
        {
            return BadRequest(ErrorMessages.InvalidId);
        }

        try
        {
            var itemToDelete = await categorias.GetAsync(id, cancellationToken);

            if (itemToDelete is null)
            {
                return NotFound();
            }

            if (itemToDelete.Protegida)
            {
                ViewData[ViewDataKeys.ErrorMessage] = ErrorMessages.ProtectedCategory;
            }
            else if (showError.GetValueOrDefault())
            {
                ViewData[ViewDataKeys.ErrorMessage] = ErrorMessages.GenericActionError;
            }

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
            logger.LogError(ex, "Error al cargar la categoría con id {CategoriaId} para eliminación.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(
        int id,
        CancellationToken cancellationToken
    )
    {
        if (!IsValidId(id))
        {
            return BadRequest(ErrorMessages.InvalidId);
        }

        try
        {
            var itemToDelete = await categorias.GetAsync(id, cancellationToken);

            if (itemToDelete is null)
            {
                return NotFound();
            }

            if (itemToDelete.Protegida)
            {
                return RedirectToAction(nameof(Eliminar), new { id, showError = true });
            }

            await categorias.DeleteAsync(id, cancellationToken);
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
            logger.LogError(ex, "Error al eliminar la categoría con id {CategoriaId}.", id);
            return RedirectToAction(nameof(Eliminar), new { id, showError = true });
        }
    }

    private static bool IsValidId(int id)
    {
        return id > 0;
    }

    private static void NormalizeCategoria(Categoria categoria)
    {
        categoria.Nombre = categoria.Nombre.Trim();
    }
}