using frontendnet.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = AppRoles.User)]
public class CarritoController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}