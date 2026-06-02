namespace frontendnet.Models.Validation;

public static class CategoryValidation
{
    public const int NameMinLength = 2;
    public const int NameMaxLength = 80;

    public const string RequiredMessage = "El campo {0} es obligatorio.";
    public const string LengthMessage = "El campo {0} debe tener entre {2} y {1} caracteres.";
    public const string InvalidIdMessage = "El identificador debe ser válido.";
    public const string InvalidNameMessage = "El campo {0} no puede estar vacío.";

    public const string IdDisplayName = "Id";
    public const string NameDisplayName = "Nombre";
    public const string DeletableDisplayName = "Eliminable";
}