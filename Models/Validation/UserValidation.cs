namespace frontendnet.Models.Validation;

public static class UserValidation
{
    public const int IdMaxLength = 64;
    public const int EmailMaxLength = 254;
    public const int NameMaxLength = 120;
    public const int RoleMaxLength = 50;

    public const string RequiredMessage = "El campo {0} es obligatorio.";
    public const string InvalidEmailMessage = "El campo {0} no es un correo válido.";
    public const string MaxLengthMessage = "El campo {0} no puede exceder {1} caracteres.";

    public const string IdDisplayName = "Id";
    public const string EmailDisplayName = "Correo electrónico";
    public const string NameDisplayName = "Nombre";
    public const string RoleDisplayName = "Rol";
}