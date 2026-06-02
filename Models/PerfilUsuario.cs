using System.ComponentModel.DataAnnotations;

namespace frontendnet.Models;

public class PerfilUsuario
{
    [Display(Name = "Correo electrónico")]
    public required string Email { get; set; }

    [Display(Name = "Nombre")]
    public required string Nombre { get; set; }

    [Display(Name = "Rol")]
    public required string Rol { get; set; }

    [Display(Name = "Tiempo restante")]
    public string? TiempoRestante { get; set; }
}