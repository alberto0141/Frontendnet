using System.Net;
using frontendnet.Models;
using frontendnet.Models.Validation;
using frontendnet.Security;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = AppRoles.Administrator)]
public class ArchivosController(
    ArchivosClientService archivos,
    IConfiguration configuration,
    ILogger<ArchivosController> logger
) : Controller
{
    private static class ConfigurationKeys
    {
        public const string UrlWebApi = "UrlWebAPI";
    }

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
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        SetUrlWebApi();

        try
        {
            var lista = await archivos.GetAsync(cancellationToken);
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
            logger.LogError(ex, "Error al obtener la lista de archivos.");
            ModelState.AddModelError(string.Empty, ErrorMessages.GenericLoadError);
            return View(new List<Archivo>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id, CancellationToken cancellationToken)
    {
        if (!IsValidId(id))
        {
            return BadRequest(ErrorMessages.InvalidId);
        }

        SetUrlWebApi();

        try
        {
            var item = await archivos.GetAsync(id, cancellationToken);

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
            logger.LogError(ex, "Error al consultar el detalle del archivo con id {ArchivoId}.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public IActionResult Crear()
    {
        SetUrlWebApi();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(FileValidation.MaxFileSizeBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = FileValidation.MaxFileSizeBytes)]
    public async Task<IActionResult> CrearAsync(
        Upload itemToCreate,
        CancellationToken cancellationToken
    )
    {
        SetUrlWebApi();

        ValidateUploadFile(itemToCreate);

        if (!ModelState.IsValid)
        {
            return View(itemToCreate);
        }

        try
        {
            await archivos.PostAsync(itemToCreate, cancellationToken);
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
            logger.LogError(ex, "Error al crear un archivo.");
            ModelState.AddModelError(nameof(Upload.Portada), ErrorMessages.GenericActionError);
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

        SetUrlWebApi();

        try
        {
            var item = await archivos.GetAsync(id, cancellationToken);

            if (item is null)
            {
                return NotFound();
            }

            var model = new Upload
            {
                ArchivoId = item.ArchivoId,
                Nombre = item.Nombre,
                Portada = null!
            };

            SetFileViewData(model);

            return View(model);
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
            logger.LogError(ex, "Error al cargar el archivo con id {ArchivoId} para edición.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(FileValidation.MaxFileSizeBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = FileValidation.MaxFileSizeBytes)]
    public async Task<IActionResult> EditarAsync(
        int id,
        Upload itemToEdit,
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

        itemToEdit.ArchivoId = id;

        SetUrlWebApi();
        SetFileViewData(itemToEdit);
        ValidateUploadFile(itemToEdit);

        if (!ModelState.IsValid)
        {
            return View(itemToEdit);
        }

        try
        {
            await archivos.PutAsync(itemToEdit, cancellationToken);
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
            logger.LogError(ex, "Error al editar el archivo con id {ArchivoId}.", id);
            ModelState.AddModelError(nameof(Upload.Portada), ErrorMessages.GenericActionError);
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

        SetUrlWebApi();

        try
        {
            var itemToDelete = await archivos.GetAsync(id, cancellationToken);

            if (itemToDelete is null)
            {
                return NotFound();
            }

            if (showError.GetValueOrDefault())
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
            logger.LogError(ex, "Error al cargar el archivo con id {ArchivoId} para eliminación.", id);
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

        SetUrlWebApi();

        try
        {
            await archivos.DeleteAsync(id, cancellationToken);
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
            logger.LogError(ex, "Error al eliminar el archivo con id {ArchivoId}.", id);
            return RedirectToAction(nameof(Eliminar), new { id, showError = true });
        }
    }

    private static bool IsValidId(int id)
    {
        return id > 0;
    }

    private void ValidateUploadFile(Upload? upload)
    {
        if (upload?.Portada is null || upload.Portada.Length <= 0)
        {
            ModelState.AddModelError(nameof(Upload.Portada), FileValidation.InvalidFileMessage);
            return;
        }

        if (upload.Portada.Length > FileValidation.MaxFileSizeBytes)
        {
            ModelState.AddModelError(nameof(Upload.Portada), FileValidation.InvalidFileSizeMessage);
        }

        var extension = Path.GetExtension(upload.Portada.FileName).ToLowerInvariant();

        if (!FileValidation.AllowedExtensions.Contains(extension) ||
            !FileValidation.AllowedContentTypes.Contains(upload.Portada.ContentType))
        {
            ModelState.AddModelError(nameof(Upload.Portada), FileValidation.InvalidFileTypeMessage);
        }
    }

    private void SetFileViewData(Upload upload)
    {
        ViewBag.ArchivoId = upload.ArchivoId;
        ViewBag.Nombre = upload.Nombre;
    }

    private void SetUrlWebApi()
    {
        var configuredUrl = configuration[ConfigurationKeys.UrlWebApi];

        if (Uri.TryCreate(configuredUrl, UriKind.Absolute, out var uri))
        {
            ViewBag.Url = uri.ToString().TrimEnd('/');
            return;
        }

        ViewBag.Url = string.Empty;
    }
}