using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using frontendnet.Models.Validation;

namespace frontendnet.Models;

public class Bitacora
{
    [JsonPropertyName("id")]
    [Display(Name = AuditLogValidation.IdDisplayName)]
    [Range(1, int.MaxValue, ErrorMessage = AuditLogValidation.InvalidIdMessage)]
    public int? BitacoraId { get; set; }

    [Display(Name = AuditLogValidation.ActionDisplayName)]
    [StringLength(AuditLogValidation.ActionMaxLength, ErrorMessage = AuditLogValidation.MaxLengthMessage)]
    public string? Accion { get; set; }

    [Display(Name = AuditLogValidation.ElementIdDisplayName)]
    [StringLength(AuditLogValidation.ElementIdMaxLength, ErrorMessage = AuditLogValidation.MaxLengthMessage)]
    public string? ElementoId { get; set; }

    [Display(Name = AuditLogValidation.IpDisplayName)]
    [StringLength(AuditLogValidation.IpMaxLength, ErrorMessage = AuditLogValidation.MaxLengthMessage)]
    public string? IP { get; set; }

    [Display(Name = AuditLogValidation.UserDisplayName)]
    [StringLength(AuditLogValidation.UserMaxLength, ErrorMessage = AuditLogValidation.MaxLengthMessage)]
    public string? Usuario { get; set; }

    [Display(Name = AuditLogValidation.DateDisplayName)]
    public DateTime? Fecha { get; set; }
}