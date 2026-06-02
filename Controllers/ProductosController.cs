using System.Net;
using frontendnet.Models;
using frontendnet.Security;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace frontendnet;

[Authorize(Roles = AppRoles.AuthenticatedRoles)]
public class ProductosController(
    ProductosClientService productos,
    CategoriasClientService categorias,
    ArchivosClientService archivos,
    IConfiguration configuration,
    ILogger<ProductosController> logger
) : Controller
{
    private const int SearchMaxLength = 120;

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
        public const string ProductoId = "ProductoId";
    }

    private static class ErrorMessages
    {
        public const string GenericActionError = "No ha sido posible realizar la acción. Inténtelo nuevamente.";
        public const string GenericLoadError = "No ha sido posible cargar la información. Inténtelo nuevamente.";
        public const string InvalidId = "El identificador solicitado no es válido.";
        public const string InvalidPoster = "La URL de la imagen no es válida.";
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? s,
        CancellationToken cancellationToken
    )
    {
        SetUrlWebApi();

        var search = NormalizeSearch(s);

        ViewBag.Search = search;
        ViewBag.SoloAdmin = User.IsInRole(AppRoles.Administrator);

        try
        {
            var lista = await productos.GetAsync(search, cancellationToken);
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
            logger.LogError(ex, "Error al obtener la lista de productos.");
            ModelState.AddModelError(string.Empty, ErrorMessages.GenericLoadError);
            return View(new List<Producto>());
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

        SetUrlWebApi();

        try
        {
            var item = await productos.GetAsync(id, cancellationToken);

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
            logger.LogError(ex, "Error al consultar el producto con id {ProductoId}.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<IActionResult> Crear(CancellationToken cancellationToken)
    {
        SetUrlWebApi();

        try
        {
            await ArchivosDropDownListAsync(null, cancellationToken);
            return View();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al cargar la vista de creación de producto.");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Administrator)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        Producto itemToCreate,
        CancellationToken cancellationToken
    )
    {
        if (itemToCreate is null)
        {
            return BadRequest();
        }

        SetUrlWebApi();
        NormalizeProducto(itemToCreate);

        if (!ModelState.IsValid)
        {
            await ArchivosDropDownListAsync(itemToCreate.ArchivoId, cancellationToken);
            return View(itemToCreate);
        }

        try
        {
            await productos.PostAsync(itemToCreate, cancellationToken);
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
            logger.LogError(ex, "Error al crear producto.");
            ModelState.AddModelError(nameof(Producto.Titulo), ErrorMessages.GenericActionError);
            await ArchivosDropDownListAsync(itemToCreate.ArchivoId, cancellationToken);
            return View(itemToCreate);
        }
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<IActionResult> Editar(
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
            var itemToEdit = await productos.GetAsync(id, cancellationToken);

            if (itemToEdit is null)
            {
                return NotFound();
            }

            await ArchivosDropDownListAsync(itemToEdit.ArchivoId, cancellationToken);
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
            logger.LogError(ex, "Error al cargar producto con id {ProductoId} para edición.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Administrator)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(
        int id,
        Producto itemToEdit,
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

        if (id != itemToEdit.ProductoId)
        {
            return BadRequest(ErrorMessages.InvalidId);
        }

        SetUrlWebApi();
        NormalizeProducto(itemToEdit);

        if (!ModelState.IsValid)
        {
            await ArchivosDropDownListAsync(itemToEdit.ArchivoId, cancellationToken);
            return View(itemToEdit);
        }

        try
        {
            await productos.PutAsync(itemToEdit, cancellationToken);
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
            logger.LogError(ex, "Error al editar producto con id {ProductoId}.", id);
            ModelState.AddModelError(nameof(Producto.Titulo), ErrorMessages.GenericActionError);
            await ArchivosDropDownListAsync(itemToEdit.ArchivoId, cancellationToken);
            return View(itemToEdit);
        }
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Administrator)]
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
            var itemToDelete = await productos.GetAsync(id, cancellationToken);

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
            logger.LogError(ex, "Error al cargar producto con id {ProductoId} para eliminación.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Administrator)]
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
            await productos.DeleteAsync(id, cancellationToken);
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
            logger.LogError(ex, "Error al eliminar producto con id {ProductoId}.", id);
            return RedirectToAction(nameof(Eliminar), new { id, showError = true });
        }
    }

    [AcceptVerbs("GET", "POST")]
    [Authorize(Roles = AppRoles.Administrator)]
    public IActionResult ValidaPoster(string? poster)
    {
        if (string.IsNullOrWhiteSpace(poster))
        {
            return Json(false);
        }

        var normalizedPoster = poster.Trim();

        if (string.Equals(normalizedPoster, "N/A", StringComparison.OrdinalIgnoreCase))
        {
            return Json(true);
        }

        var isValidUri = Uri.TryCreate(normalizedPoster, UriKind.Absolute, out var uri) &&
                         (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);

        return Json(isValidUri);
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<IActionResult> Categorias(
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
            var itemToView = await productos.GetAsync(id, cancellationToken);

            if (itemToView is null)
            {
                return NotFound();
            }

            ViewData[ViewDataKeys.ProductoId] = itemToView.ProductoId;
            return View(itemToView);
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
            logger.LogError(ex, "Error al cargar categorías del producto con id {ProductoId}.", id);
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<IActionResult> CategoriasAgregar(
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
            var producto = await productos.GetAsync(id, cancellationToken);

            if (producto is null)
            {
                return NotFound();
            }

            await CategoriasDropDownListAsync(null, cancellationToken);

            var itemToView = new ProductoCategoria
            {
                Producto = producto
            };

            return View(itemToView);
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
            logger.LogError(ex, "Error al cargar vista para agregar categoría al producto {ProductoId}.", id);
            return RedirectToAction(nameof(Categorias), new { id });
        }
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Administrator)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CategoriasAgregar(
        int id,
        int categoriaId,
        CancellationToken cancellationToken
    )
    {
        if (!IsValidId(id) || !IsValidId(categoriaId))
        {
            return BadRequest(ErrorMessages.InvalidId);
        }

        SetUrlWebApi();

        Producto? producto = null;

        try
        {
            producto = await productos.GetAsync(id, cancellationToken);

            if (producto is null)
            {
                return NotFound();
            }

            var categoria = await categorias.GetAsync(categoriaId, cancellationToken);

            if (categoria is null)
            {
                return NotFound();
            }

            await productos.PostAsync(id, categoriaId, cancellationToken);
            return RedirectToAction(nameof(Categorias), new { id });
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
            logger.LogError(
                ex,
                "Error al agregar categoría {CategoriaId} al producto {ProductoId}.",
                categoriaId,
                id
            );

            ModelState.AddModelError(nameof(ProductoCategoria.CategoriaId), ErrorMessages.GenericActionError);
            await CategoriasDropDownListAsync(categoriaId, cancellationToken);

            return View(new ProductoCategoria { Producto = producto });
        }
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Administrator)]
    public async Task<IActionResult> CategoriasRemover(
        int id,
        int categoriaId,
        bool? showError = false,
        CancellationToken cancellationToken = default
    )
    {
        if (!IsValidId(id) || !IsValidId(categoriaId))
        {
            return BadRequest(ErrorMessages.InvalidId);
        }

        SetUrlWebApi();

        try
        {
            var producto = await productos.GetAsync(id, cancellationToken);

            if (producto is null)
            {
                return NotFound();
            }

            var categoria = await categorias.GetAsync(categoriaId, cancellationToken);

            if (categoria is null)
            {
                return NotFound();
            }

            if (showError.GetValueOrDefault())
            {
                ViewData[ViewDataKeys.ErrorMessage] = ErrorMessages.GenericActionError;
            }

            var itemToView = new ProductoCategoria
            {
                Producto = producto,
                CategoriaId = categoriaId,
                Nombre = categoria.Nombre
            };

            return View(itemToView);
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
            logger.LogError(
                ex,
                "Error al cargar vista para remover categoría {CategoriaId} del producto {ProductoId}.",
                categoriaId,
                id
            );

            return RedirectToAction(nameof(Categorias), new { id });
        }
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Administrator)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CategoriasRemover(
        int id,
        int categoriaId,
        CancellationToken cancellationToken
    )
    {
        if (!IsValidId(id) || !IsValidId(categoriaId))
        {
            return BadRequest(ErrorMessages.InvalidId);
        }

        try
        {
            await productos.DeleteAsync(id, categoriaId, cancellationToken);
            return RedirectToAction(nameof(Categorias), new { id });
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
            logger.LogError(
                ex,
                "Error al remover categoría {CategoriaId} del producto {ProductoId}.",
                categoriaId,
                id
            );

            return RedirectToAction(nameof(CategoriasRemover), new { id, categoriaId, showError = true });
        }
    }

    private static bool IsValidId(int id)
    {
        return id > 0;
    }

    private static string? NormalizeSearch(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return null;
        }

        var normalizedSearch = search.Trim();

        return normalizedSearch.Length > SearchMaxLength
            ? normalizedSearch[..SearchMaxLength]
            : normalizedSearch;
    }

    private static void NormalizeProducto(Producto producto)
    {
        producto.Titulo = producto.Titulo.Trim();
        producto.Descripcion = producto.Descripcion.Trim();
    }

    private async Task CategoriasDropDownListAsync(
        object? itemSeleccionado,
        CancellationToken cancellationToken
    )
    {
        var listado = await categorias.GetAsync(cancellationToken);
        ViewBag.Categoria = new SelectList(listado, "CategoriaId", "Nombre", itemSeleccionado);
    }

    private async Task ArchivosDropDownListAsync(
        object? itemSeleccionado,
        CancellationToken cancellationToken
    )
    {
        var listado = await archivos.GetAsync(cancellationToken);
        ViewBag.Archivo = new SelectList(listado, "ArchivoId", "Nombre", itemSeleccionado);
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