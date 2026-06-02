using System.ComponentModel.DataAnnotations;
using frontendnet.Models.Validation;

namespace frontendnet.Models;

public class Rol
{
    [Required(ErrorMessage = RoleValidation.RequiredMessage)]
    [StringLength(RoleValidation.IdMaxLength, ErrorMessage = RoleValidation.MaxLengthMessage)]
    [Display(Name = RoleValidation.IdDisplayName)]
    public required string Id { get; set; }

    [Required(ErrorMessage = RoleValidation.RequiredMessage)]
    [StringLength(RoleValidation.NameMaxLength, ErrorMessage = RoleValidation.MaxLengthMessage)]
    [Display(Name = RoleValidation.NameDisplayName)]
    public required string Nombre { get; set; }
}