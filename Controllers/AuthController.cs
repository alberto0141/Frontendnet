using System.Net;
using System.Security.Claims;
using frontendnet.Models;
using frontendnet.Security;
using frontendnet.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

public class AuthController(
    AuthClientService auth,
    ILogger<AuthController> logger
) : Controller
{
    private static class ActionNames
    {
        public const string Index = "Index";
        public const string Registro = "Registro";
    }

    private static class TempDataKeys
    {
        public const string SuccessMessage = "SuccessMessage";
    }

    private static class ControllerNames
    {
        public const string Auth = "Auth";
        public const string Home = "Home";
        public const string Products = "Productos";
    }

    private static class ErrorMessages
    {
        public const string InvalidCredentials = "Credenciales no válidas. Inténtelo nuevamente.";
        public const string InvalidAuthResponse = "No ha sido posible iniciar sesión. Inténtelo nuevamente.";
        public const string EmailAlreadyRegistered = "El correo electrónico ya está registrado.";
        public const string RegistrationRateLimit = "Demasiados intentos. Inténtelo de nuevo más tarde.";
        public const string RegistrationFailed = "No ha sido posible crear la cuenta. Inténtelo nuevamente.";
    }

    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByRole(User.FindFirstValue(ClaimTypes.Role));
        }

        return View(new Login
        {
            Email = string.Empty,
            Password = string.Empty
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> IndexAsync(
        Login model,
        CancellationToken cancellationToken
    )
    {
        if (model is null)
        {
            return BadRequest();
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByRole(User.FindFirstValue(ClaimTypes.Role));
        }

        NormalizeLoginModel(model);

        if (!ModelState.IsValid)
        {
            return View(nameof(Index), model);
        }

        try
        {
            var token = await auth.ObtenerTokenAsync(
                model.Email,
                model.Password,
                cancellationToken
            );

            if (!IsValidAuthUser(token))
            {
                logger.LogWarning("El backend devolvió una respuesta de autenticación incompleta.");
                ModelState.AddModelError(nameof(Login.Email), ErrorMessages.InvalidAuthResponse);
                return View(nameof(Index), model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, token.Email),
                new(ClaimTypes.GivenName, token.Nombre),
                new(ClaimTypes.Role, token.Rol),
                new(AppClaims.Jwt, token.Jwt)
            };

            await auth.IniciaSesionAsync(claims, cancellationToken);

            return RedirectByRole(token.Rol);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Intento de inicio de sesión con credenciales inválidas.");
            ModelState.AddModelError(nameof(Login.Email), ErrorMessages.InvalidCredentials);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error al comunicarse con el backend durante el inicio de sesión.");
            ModelState.AddModelError(nameof(Login.Email), ErrorMessages.InvalidCredentials);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado durante el inicio de sesión.");
            ModelState.AddModelError(nameof(Login.Email), ErrorMessages.InvalidCredentials);
        }

        return View(nameof(Index), model);
    }

    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Registro()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByRole(User.FindFirstValue(ClaimTypes.Role));
        }

        return View(new RegistroPublico
        {
            Email = string.Empty,
            Nombre = string.Empty,
            Password = string.Empty,
            ConfirmarPassword = string.Empty
        });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> RegistroAsync(
        RegistroPublico model,
        CancellationToken cancellationToken
    )
    {
        if (model is null)
        {
            return BadRequest();
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByRole(User.FindFirstValue(ClaimTypes.Role));
        }

        NormalizeRegistroModel(model);

        if (!ModelState.IsValid)
        {
            return View(ActionNames.Registro, model);
        }

        try
        {
            await auth.RegistrarAsync(model, cancellationToken);

            TempData[TempDataKeys.SuccessMessage] =
                "Cuenta creada exitosamente. Ahora puede iniciar sesión.";

            return RedirectToAction(ActionNames.Index, ControllerNames.Auth);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
            logger.LogWarning("Intento de registro con correo ya existente.");
            ModelState.AddModelError(nameof(RegistroPublico.Email), ErrorMessages.EmailAlreadyRegistered);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
        {
            logger.LogWarning("Rate limit alcanzado durante el registro.");
            ModelState.AddModelError(string.Empty, ErrorMessages.RegistrationRateLimit);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Error de comunicación durante el registro.");
            ModelState.AddModelError(string.Empty, ErrorMessages.RegistrationFailed);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado durante el registro.");
            ModelState.AddModelError(string.Empty, ErrorMessages.RegistrationFailed);
        }

        return View(ActionNames.Registro, model);
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.AuthenticatedRoles)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SalirAsync()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return RedirectToAction(ActionNames.Index, ControllerNames.Auth);
    }

    private static void NormalizeLoginModel(Login model)
    {
        model.Email = model.Email.Trim().ToLowerInvariant();
    }

    private static void NormalizeRegistroModel(RegistroPublico model)
    {
        model.Email = model.Email.Trim().ToLowerInvariant();
        model.Nombre = model.Nombre.Trim();
    }

    private static bool IsValidAuthUser(AuthUser token)
    {
        return token is not null &&
            !string.IsNullOrWhiteSpace(token.Email) &&
            !string.IsNullOrWhiteSpace(token.Nombre) &&
            !string.IsNullOrWhiteSpace(token.Rol) &&
            !string.IsNullOrWhiteSpace(token.Jwt);
    }

    private static IActionResult RedirectByRole(string? role)
    {
        if (string.Equals(role, AppRoles.Administrator, StringComparison.Ordinal))
        {
            return new RedirectToActionResult(
                ActionNames.Index,
                ControllerNames.Products,
                routeValues: null
            );
        }

        return new RedirectToActionResult(
            ActionNames.Index,
            ControllerNames.Home,
            routeValues: null
        );
    }
}