using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using frontendnet.Models.Validation;

namespace frontendnet.Models;

public class Producto
{
    [Display(Name = ProductValidation.IdDisplayName)]
    [JsonPropertyName("id")]
    public int ProductoId { get; set; }

    [Required(ErrorMessage = ProductValidation.RequiredMessage)]
    [StringLength(ProductValidation.TitleMaxLength, ErrorMessage = ProductValidation.MaxLengthMessage)]
    [RegularExpression(@"^[^<>]+$", ErrorMessage = ProductValidation.NoHtmlCharsMessage)]
    [Display(Name = ProductValidation.TitleDisplayName)]
    [JsonPropertyName("titulo")]
    public required string Titulo { get; set; }

    [Required(ErrorMessage = ProductValidation.RequiredMessage)]
    [StringLength(ProductValidation.DescriptionMaxLength, ErrorMessage = ProductValidation.MaxLengthMessage)]
    [RegularExpression(@"^[^<>]+$", ErrorMessage = ProductValidation.NoHtmlCharsMessage)]
    [DataType(DataType.MultilineText)]
    [Display(Name = ProductValidation.DescriptionDisplayName)]
    [JsonPropertyName("descripcion")]
    public required string Descripcion { get; set; }

    [Range(
        typeof(decimal),
        ProductValidation.PriceMinValue,
        ProductValidation.PriceMaxValue,
        ErrorMessage = ProductValidation.PriceRangeMessage
    )]
    [DataType(DataType.Currency)]
    [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
    [Display(Name = ProductValidation.PriceDisplayName)]
    [JsonPropertyName("precio")]
    public decimal Precio { get; set; }

    [Display(Name = ProductValidation.CoverDisplayName)]
    [JsonPropertyName("archivoid")]
    public int? ArchivoId { get; set; }

    [Display(Name = ProductValidation.DeletableDisplayName)]
    [JsonPropertyName("protegida")]
    public bool Protegida { get; set; } = false;

    [JsonPropertyName("categorias")]
    public ICollection<Categoria>? Categorias { get; set; }
}