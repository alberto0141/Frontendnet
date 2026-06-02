using System.Net;
using frontendnet.Models;
using frontendnet.Security;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = AppRoles.AuthenticatedRoles)]
public class ComprarController(
    ProductosClientService productos,
    IConfiguration configuration,
    ILogger<ComprarController> logger
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

    private static class ErrorMessages
    {
        public const string GenericLoadError = "No ha sido posible cargar los productos. Inténtelo nuevamente.";
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
            logger.LogError(ex, "Error al obtener productos para comprar.");

            ModelState.AddModelError(string.Empty, ErrorMessages.GenericLoadError);

            return View(new List<Producto>());
        }
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