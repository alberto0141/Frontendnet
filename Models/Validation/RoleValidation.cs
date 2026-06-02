namespace frontendnet.Models.Validation;

public static class RoleValidation
{
    public const int IdMaxLength = 50;
    public const int NameMaxLength = 50;

    public const string RequiredMessage = "El campo {0} es obligatorio.";
    public const string MaxLengthMessage = "El campo {0} no puede exceder {1} caracteres.";

    public const string IdDisplayName = "Id";
    public const string NameDisplayName = "Rol";
}