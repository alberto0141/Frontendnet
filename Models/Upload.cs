using System.ComponentModel.DataAnnotations;
using frontendnet.Models.Validation;
using Microsoft.AspNetCore.Http;

namespace frontendnet.Models;

public class Upload : IValidatableObject
{
    [Display(Name = FileValidation.IdDisplayName)]
    [Range(1, int.MaxValue, ErrorMessage = FileValidation.InvalidIdMessage)]
    public int? ArchivoId { get; set; }

    [StringLength(FileValidation.FileNameMaxLength, ErrorMessage = FileValidation.MaxLengthMessage)]
    [Display(Name = FileValidation.FileNameDisplayName)]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = FileValidation.RequiredMessage)]
    [DataType(DataType.Upload)]
    [Display(Name = FileValidation.FileDisplayName)]
    public required IFormFile Portada { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Portada is null || Portada.Length <= 0)
        {
            yield return new ValidationResult(
                FileValidation.InvalidFileMessage,
                [nameof(Portada)]
            );

            yield break;
        }

        if (Portada.Length > FileValidation.MaxFileSizeBytes)
        {
            yield return new ValidationResult(
                FileValidation.InvalidFileSizeMessage,
                [nameof(Portada)]
            );
        }

        var extension = Path.GetExtension(Portada.FileName).ToLowerInvariant();

        if (!FileValidation.AllowedExtensions.Contains(extension) ||
            !FileValidation.AllowedContentTypes.Contains(Portada.ContentType))
        {
            yield return new ValidationResult(
                FileValidation.InvalidFileTypeMessage,
                [nameof(Portada)]
            );
        }
    }
}