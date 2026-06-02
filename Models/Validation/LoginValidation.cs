namespace frontendnet.Models.Validation;

public static class LoginValidation
{
    public const int EmailMaxLength = 254;
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 128;

    public const string RequiredMessage = "El campo {0} es obligatorio.";
    public const string InvalidEmailMessage = "El campo {0} no es un correo válido.";
    public const string MaxLengthMessage = "El campo {0} no puede exceder {1} caracteres.";
    public const string RangeLengthMessage = "El campo {0} debe tener entre {2} y {1} caracteres.";

    public const string EmailDisplayName = "Correo electrónico";
    public const string PasswordDisplayName = "Contraseña";
}