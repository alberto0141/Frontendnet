using System.Net;
using frontendnet.Extensions;
using frontendnet.Models;
using frontendnet.Security;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = AppRoles.User)]
public class CarritoController(
    ProductosClientService productos,
    PedidosClientService pedidos,
    ILogger<CarritoController> logger
) : Controller
{
    private const string CarritoKey = "carrito";
    private const int CantidadMin = 1;
    private const int CantidadMax = 99;

    private static class ActionNames
    {
        public const string Logout = "Salir";
    }

    private static class ControllerNames
    {
        public const string Auth = "Auth";
        public const string Pedidos = "Pedidos";
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new ResumenCarrito { Items = LeerCarrito() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Agregar(int id, CancellationToken cancellationToken)
    {
        if (!EsIdValido(id))
        {
            return BadRequest();
        }

        Producto? producto;

        try
        {
            producto = await productos.GetAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al consultar producto {Id} para el carrito.", id);
            TempData["Error"] = "No fue posible agregar el producto. Inténtelo nuevamente.";
            return RedirectToAction(nameof(Index));
        }

        if (producto is null)
        {
            TempData["Error"] = "El producto no existe.";
            return RedirectToAction(nameof(Index));
        }

        var carrito = LeerCarrito();
        AgregarOIncrementar(carrito, producto);
        GuardarCarrito(carrito);

        TempData["Exito"] = $"'{TruncatarTitulo(producto.Titulo)}' agregado al carrito.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Incrementar(int id) => AjustarCantidad(id, +1);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Decrementar(int id) => AjustarCantidad(id, -1);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(int id)
    {
        if (!EsIdValido(id))
        {
            return BadRequest();
        }

        var carrito = LeerCarrito();
        carrito.RemoveAll(i => i.ProductoId == id);
        GuardarCarrito(carrito);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Vaciar()
    {
        GuardarCarrito([]);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmarCompra(CancellationToken cancellationToken)
    {
        var carrito = LeerCarrito();

        if (carrito.Count == 0)
        {
            TempData["Error"] = "El carrito está vacío. Agrega productos antes de confirmar.";
            return RedirectToAction(nameof(Index));
        }

        Pedido? pedido;

        try
        {
            pedido = await pedidos.CrearAsync(
                new ResumenCarrito { Items = carrito },
                cancellationToken
            );
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            return RedirectToAction(ActionNames.Logout, ControllerNames.Auth);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
        {
            logger.LogWarning(ex, "Pedido rechazado por validación del backend.");
            TempData["Error"] = "Algunos productos no están disponibles. Revisa tu carrito.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al crear pedido.");
            TempData["Error"] = "No fue posible procesar el pedido. Inténtelo nuevamente.";
            return RedirectToAction(nameof(Index));
        }

        GuardarCarrito([]);
        TempData["Exito"] = "¡Pedido creado correctamente!";

        if (pedido?.Id > 0)
        {
            return RedirectToAction("Detalle", ControllerNames.Pedidos, new { id = pedido.Id });
        }

        return RedirectToAction("Index", ControllerNames.Pedidos);
    }

    private List<CarritoItem> LeerCarrito() =>
        HttpContext.Session.GetJson<List<CarritoItem>>(CarritoKey) ?? [];

    private void GuardarCarrito(List<CarritoItem> carrito) =>
        HttpContext.Session.SetJson(CarritoKey, carrito);

    private static CarritoItem? BuscarItem(List<CarritoItem> carrito, int id) =>
        carrito.FirstOrDefault(i => i.ProductoId == id);

    private IActionResult AjustarCantidad(int id, int delta)
    {
        if (!EsIdValido(id))
        {
            return BadRequest();
        }

        var carrito = LeerCarrito();
        var item = BuscarItem(carrito, id);

        if (item is not null)
        {
            var nueva = item.Cantidad + delta;
            if (EsCantidadValida(nueva))
            {
                item.Cantidad = nueva;
                GuardarCarrito(carrito);
            }
        }

        return RedirectToAction(nameof(Index));
    }

    private static void AgregarOIncrementar(List<CarritoItem> carrito, Producto producto)
    {
        var item = BuscarItem(carrito, producto.ProductoId);

        if (item is null)
        {
            carrito.Add(new CarritoItem
            {
                ProductoId = producto.ProductoId,
                Titulo = producto.Titulo,
                Precio = producto.Precio,
                Cantidad = 1,
                ArchivoId = producto.ArchivoId
            });
        }
        else
        {
            item.Cantidad = Math.Min(item.Cantidad + 1, CantidadMax);
        }
    }

    private static bool EsIdValido(int id) => id > 0;

    private static bool EsCantidadValida(int cantidad) =>
        cantidad >= CantidadMin && cantidad <= CantidadMax;

    private static string TruncatarTitulo(string titulo) =>
        titulo.Length > 60 ? titulo[..60] + "..." : titulo;
}
