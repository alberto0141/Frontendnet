namespace frontendnet.Models.Validation;

public static class ProductCategoryValidation
{
    public const int NameMaxLength = 120;

    public const string RequiredMessage = "El campo {0} es obligatorio.";
    public const string InvalidCategoryMessage = "Debe seleccionar una categoría válida.";
    public const string MaxLengthMessage = "El campo {0} no puede exceder {1} caracteres.";

    public const string CategoryDisplayName = "Categoría";
    public const string NameDisplayName = "Nombre";
}