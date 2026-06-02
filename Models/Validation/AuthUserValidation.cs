namespace frontendnet.Models.Validation;

public static class AuthUserValidation
{
    public const int EmailMaxLength = 150;
    public const int NameMaxLength = 120;
    public const int RoleMaxLength = 50;
    public const int JwtMaxLength = 4096;

    public const string RequiredMessage = "El campo {0} es obligatorio.";
    public const string MaxLengthMessage = "El campo {0} no puede exceder {1} caracteres.";
    public const string InvalidEmailMessage = "El campo {0} no es un correo válido.";

    public const string EmailDisplayName = "Correo electrónico";
    public const string NameDisplayName = "Nombre";
    public const string RoleDisplayName = "Rol";
    public const string JwtDisplayName = "Token";
}