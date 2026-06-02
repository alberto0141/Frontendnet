namespace frontendnet.Models.Validation;

public static class RegistroPublicoValidation
{
    public const int EmailMaxLength = 150;
    public const int NameMaxLength = 120;
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 72;

    public const string PasswordPattern =
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])\S+$";

    public const string RequiredMessage = "El campo {0} es obligatorio.";
    public const string InvalidEmailMessage = "El campo {0} no es un correo válido.";
    public const string MaxLengthMessage = "El campo {0} no puede exceder {1} caracteres.";
    public const string PasswordLengthMessage = "El campo {0} debe tener entre {2} y {1} caracteres.";
    public const string PasswordPatternMessage = "La contraseña debe incluir mayúscula, minúscula, número y carácter especial, sin espacios.";
    public const string PasswordMismatchMessage = "Las contraseñas no coinciden.";

    public const string EmailDisplayName = "Correo electrónico";
    public const string NameDisplayName = "Nombre";
    public const string PasswordDisplayName = "Contraseña";
    public const string ConfirmPasswordDisplayName = "Confirmar contraseña";
}
