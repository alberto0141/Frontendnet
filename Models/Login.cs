using System.ComponentModel.DataAnnotations;
using frontendnet.Models.Validation;

namespace frontendnet.Models;

public class Login
{
    [Required(ErrorMessage = LoginValidation.RequiredMessage)]
    [EmailAddress(ErrorMessage = LoginValidation.InvalidEmailMessage)]
    [StringLength(LoginValidation.EmailMaxLength, ErrorMessage = LoginValidation.MaxLengthMessage)]
    [Display(Name = LoginValidation.EmailDisplayName)]
    public required string Email { get; set; }

    [Required(ErrorMessage = LoginValidation.RequiredMessage)]
    [StringLength(
        LoginValidation.PasswordMaxLength,
        MinimumLength = LoginValidation.PasswordMinLength,
        ErrorMessage = LoginValidation.RangeLengthMessage
    )]
    [DataType(DataType.Password)]
    [Display(Name = LoginValidation.PasswordDisplayName)]
    public required string Password { get; set; }
}