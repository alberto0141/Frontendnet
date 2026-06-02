using System.ComponentModel.DataAnnotations;
using frontendnet.Models.Validation;

namespace frontendnet.Models;

public class RegistroPublico
{
    [Required(ErrorMessage = RegistroPublicoValidation.RequiredMessage)]
    [EmailAddress(ErrorMessage = RegistroPublicoValidation.InvalidEmailMessage)]
    [StringLength(RegistroPublicoValidation.EmailMaxLength, ErrorMessage = RegistroPublicoValidation.MaxLengthMessage)]
    [Display(Name = RegistroPublicoValidation.EmailDisplayName)]
    public required string Email { get; set; }

    [Required(ErrorMessage = RegistroPublicoValidation.RequiredMessage)]
    [StringLength(RegistroPublicoValidation.NameMaxLength, ErrorMessage = RegistroPublicoValidation.MaxLengthMessage)]
    [Display(Name = RegistroPublicoValidation.NameDisplayName)]
    public required string Nombre { get; set; }

    [Required(ErrorMessage = RegistroPublicoValidation.RequiredMessage)]
    [StringLength(
        RegistroPublicoValidation.PasswordMaxLength,
        MinimumLength = RegistroPublicoValidation.PasswordMinLength,
        ErrorMessage = RegistroPublicoValidation.PasswordLengthMessage
    )]
    [RegularExpression(
        RegistroPublicoValidation.PasswordPattern,
        ErrorMessage = RegistroPublicoValidation.PasswordPatternMessage
    )]
    [DataType(DataType.Password)]
    [Display(Name = RegistroPublicoValidation.PasswordDisplayName)]
    public required string Password { get; set; }

    [Required(ErrorMessage = RegistroPublicoValidation.RequiredMessage)]
    [Compare(nameof(Password), ErrorMessage = RegistroPublicoValidation.PasswordMismatchMessage)]
    [DataType(DataType.Password)]
    [Display(Name = RegistroPublicoValidation.ConfirmPasswordDisplayName)]
    public required string ConfirmarPassword { get; set; }
}
