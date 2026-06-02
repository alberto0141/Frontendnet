using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using frontendnet.Models.Validation;

namespace frontendnet.Models;

public class Archivo
{
    [JsonPropertyName("id")]
    [Display(Name = FileValidation.IdDisplayName)]
    [Range(1, int.MaxValue, ErrorMessage = FileValidation.InvalidIdMessage)]
    public int? ArchivoId { get; set; }

    [StringLength(FileValidation.MimeMaxLength, ErrorMessage = FileValidation.MaxLengthMessage)]
    [Display(Name = FileValidation.MimeDisplayName)]
    public string? Mime { get; set; }

    [StringLength(FileValidation.FileNameMaxLength, ErrorMessage = FileValidation.MaxLengthMessage)]
    [Display(Name = FileValidation.FileNameDisplayName)]
    public string? Nombre { get; set; }

    [Display(Name = FileValidation.SizeDisplayName)]
    [Range(1, FileValidation.MaxFileSizeBytes, ErrorMessage = FileValidation.InvalidSizeMessage)]
    public int? Size { get; set; }

    [Display(Name = FileValidation.StorageDisplayName)]
    public bool InDb { get; set; } = true;
}