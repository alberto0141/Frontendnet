using System.ComponentModel.DataAnnotations;
using frontendnet.Models.Validation;

namespace frontendnet.Models;

public class UsuarioPwd
{
    [Required(ErrorMessage = UserPasswordValidation.RequiredMessage)]
    [EmailAddress(ErrorMessage = UserPasswordValidation.InvalidEmailMessage)]
    [StringLength(UserPasswordValidation.EmailMaxLength, ErrorMessage = UserPasswordValidation.MaxLengthMessage)]
    [Display(Name = UserPasswordValidation.EmailDisplayName)]
    public required string Email { get; set; }

    [Required(ErrorMessage = UserPasswordValidation.RequiredMessage)]
    [StringLength(
        UserPasswordValidation.PasswordMaxLength,
        MinimumLength = UserPasswordValidation.PasswordMinLength,
        ErrorMessage = UserPasswordValidation.PasswordLengthMessage
    )]
    [RegularExpression(
        UserPasswordValidation.PasswordPattern,
        ErrorMessage = UserPasswordValidation.PasswordPatternMessage
    )]
    [DataType(DataType.Password)]
    [Display(Name = UserPasswordValidation.PasswordDisplayName)]
    public required string Password { get; set; }

    [Required(ErrorMessage = UserPasswordValidation.RequiredMessage)]
    [StringLength(UserPasswordValidation.NameMaxLength, ErrorMessage = UserPasswordValidation.MaxLengthMessage)]
    [Display(Name = UserPasswordValidation.NameDisplayName)]
    public required string Nombre { get; set; }

    [Required(ErrorMessage = UserPasswordValidation.RequiredMessage)]
    [StringLength(UserPasswordValidation.RoleMaxLength, ErrorMessage = UserPasswordValidation.MaxLengthMessage)]
    [Display(Name = UserPasswordValidation.RoleDisplayName)]
    public required string Rol { get; set; }
}