namespace frontendnet.Models.Validation;

public static class AuditLogValidation
{
    public const int ActionMaxLength = 100;
    public const int ElementIdMaxLength = 100;
    public const int IpMaxLength = 45;
    public const int UserMaxLength = 254;

    public const string MaxLengthMessage = "El campo {0} no puede exceder {1} caracteres.";
    public const string InvalidIdMessage = "El identificador debe ser válido.";

    public const string IdDisplayName = "Id";
    public const string ActionDisplayName = "Acción";
    public const string ElementIdDisplayName = "Elemento Id";
    public const string IpDisplayName = "IP";
    public const string UserDisplayName = "Usuario";
    public const string DateDisplayName = "Fecha";
}