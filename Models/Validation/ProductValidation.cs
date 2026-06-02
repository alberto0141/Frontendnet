namespace frontendnet.Models.Validation;

public static class ProductValidation
{
    public const int TitleMaxLength = 120;
    public const int DescriptionMaxLength = 1000;

    public const string PriceMinValue = "0.01";
    public const string PriceMaxValue = "999999.99";

    public const string RequiredMessage = "El campo {0} es obligatorio.";
    public const string MaxLengthMessage = "El campo {0} no puede exceder {1} caracteres.";
    public const string PriceRangeMessage = "El campo {0} debe ser mayor a 0 y tener un valor válido.";
    public const string NoHtmlCharsMessage = "El campo {0} no puede contener los caracteres < o >.";

    public const string IdDisplayName = "Id";
    public const string TitleDisplayName = "Título";
    public const string DescriptionDisplayName = "Descripción";
    public const string PriceDisplayName = "Precio";
    public const string CoverDisplayName = "Portada";
    public const string DeletableDisplayName = "Eliminable";
}