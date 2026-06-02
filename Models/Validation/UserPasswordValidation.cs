namespace frontendnet.Models.Validation;

public static class UserPasswordValidation
{
    public const int EmailMaxLength = 254;
    public const int NameMaxLength = 120;
    public const int RoleMaxLength = 50;
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 128;

    public const string PasswordPattern =
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])\S+$";

    public const string RequiredMessage = "El campo {0} es obligatorio.";
    public const string InvalidEmailMessage = "El campo {0} no es un correo válido.";
    public const string MaxLengthMessage = "El campo {0} no puede exceder {1} caracteres.";
    public const string PasswordLengthMessage = "El campo {0} debe tener entre {2} y {1} caracteres.";
    public const string PasswordPatternMessage = "La contraseña debe incluir mayúscula, minúscula, número, carácter especial y no debe contener espacios.";

    public const string EmailDisplayName = "Correo electrónico";
    public const string PasswordDisplayName = "Contraseña";
    public const string NameDisplayName = "Nombre";
    public const string RoleDisplayName = "Rol";
}