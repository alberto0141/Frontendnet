using System.ComponentModel.DataAnnotations;
using frontendnet.Models.Validation;

namespace frontendnet.Models;

public class Categoria
{
    private string _nombre = string.Empty;

    [Display(Name = CategoryValidation.IdDisplayName)]
    [Range(1, int.MaxValue, ErrorMessage = CategoryValidation.InvalidIdMessage)]
    public int? CategoriaId { get; set; }

    [Required(ErrorMessage = CategoryValidation.RequiredMessage)]
    [StringLength(
        CategoryValidation.NameMaxLength,
        MinimumLength = CategoryValidation.NameMinLength,
        ErrorMessage = CategoryValidation.LengthMessage
    )]
    [RegularExpression(@"\S.*", ErrorMessage = CategoryValidation.InvalidNameMessage)]
    [Display(Name = CategoryValidation.NameDisplayName)]
    public required string Nombre
    {
        get => _nombre;
        set => _nombre = value?.Trim() ?? string.Empty;
    }

    [Display(Name = CategoryValidation.DeletableDisplayName)]
    public bool Protegida { get; set; } = false;
}