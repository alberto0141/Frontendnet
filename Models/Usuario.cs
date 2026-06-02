using System.ComponentModel.DataAnnotations;
using frontendnet.Models.Validation;

namespace frontendnet.Models;

public class Usuario
{
    [StringLength(UserValidation.IdMaxLength, ErrorMessage = UserValidation.MaxLengthMessage)]
    [Display(Name = UserValidation.IdDisplayName)]
    public string? Id { get; set; }

    [Required(ErrorMessage = UserValidation.RequiredMessage)]
    [EmailAddress(ErrorMessage = UserValidation.InvalidEmailMessage)]
    [StringLength(UserValidation.EmailMaxLength, ErrorMessage = UserValidation.MaxLengthMessage)]
    [Display(Name = UserValidation.EmailDisplayName)]
    public required string Email { get; set; }

    [Required(ErrorMessage = UserValidation.RequiredMessage)]
    [StringLength(UserValidation.NameMaxLength, ErrorMessage = UserValidation.MaxLengthMessage)]
    [Display(Name = UserValidation.NameDisplayName)]
    public required string Nombre { get; set; }

    [Required(ErrorMessage = UserValidation.RequiredMessage)]
    [StringLength(UserValidation.RoleMaxLength, ErrorMessage = UserValidation.MaxLengthMessage)]
    [Display(Name = UserValidation.RoleDisplayName)]
    public required string Rol { get; set; }
}