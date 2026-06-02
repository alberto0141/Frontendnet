using System.ComponentModel.DataAnnotations;
using frontendnet.Models.Validation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace frontendnet.Models;

public class ProductoCategoria
{
    [Display(Name = ProductCategoryValidation.CategoryDisplayName)]
    [Required(ErrorMessage = ProductCategoryValidation.RequiredMessage)]
    [Range(1, int.MaxValue, ErrorMessage = ProductCategoryValidation.InvalidCategoryMessage)]
    public int? CategoriaId { get; set; }

    [Display(Name = ProductCategoryValidation.NameDisplayName)]
    [StringLength(ProductCategoryValidation.NameMaxLength, ErrorMessage = ProductCategoryValidation.MaxLengthMessage)]
    public string? Nombre { get; set; }

    [ValidateNever]
    public Producto? Producto { get; set; }
}