using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using frontendnet.Models.Validation;

namespace frontendnet.Models;

public class AuthUser
{
    [Required(ErrorMessage = AuthUserValidation.RequiredMessage)]
    [EmailAddress(ErrorMessage = AuthUserValidation.InvalidEmailMessage)]
    [StringLength(AuthUserValidation.EmailMaxLength, ErrorMessage = AuthUserValidation.MaxLengthMessage)]
    [Display(Name = AuthUserValidation.EmailDisplayName)]
    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [Required(ErrorMessage = AuthUserValidation.RequiredMessage)]
    [StringLength(AuthUserValidation.NameMaxLength, ErrorMessage = AuthUserValidation.MaxLengthMessage)]
    [Display(Name = AuthUserValidation.NameDisplayName)]
    [JsonPropertyName("nombre")]
    public required string Nombre { get; set; }

    [Required(ErrorMessage = AuthUserValidation.RequiredMessage)]
    [StringLength(AuthUserValidation.RoleMaxLength, ErrorMessage = AuthUserValidation.MaxLengthMessage)]
    [Display(Name = AuthUserValidation.RoleDisplayName)]
    [JsonPropertyName("rol")]
    public required string Rol { get; set; }

    [Required(ErrorMessage = AuthUserValidation.RequiredMessage)]
    [StringLength(AuthUserValidation.JwtMaxLength, ErrorMessage = AuthUserValidation.MaxLengthMessage)]
    [Display(Name = AuthUserValidation.JwtDisplayName)]
    [JsonPropertyName("jwt")]
    public required string Jwt { get; set; }
}