namespace frontendnet.Models.Validation;

public static class FileValidation
{
    public const int FileNameMaxLength = 120;
    public const int MimeMaxLength = 100;
    public const int MaxFileSizeBytes = 2 * 1024 * 1024;

    public const string RequiredMessage = "El campo {0} es obligatorio.";
    public const string MaxLengthMessage = "El campo {0} no puede exceder {1} caracteres.";
    public const string InvalidFileMessage = "Debe seleccionar un archivo válido.";
    public const string InvalidFileTypeMessage = "Solo se permiten imágenes JPG, JPEG, PNG o WebP.";
    public const string InvalidFileSizeMessage = "El archivo no debe superar 2 MB.";
    public const string InvalidIdMessage = "El identificador debe ser válido.";
    public const string InvalidSizeMessage = "El tamaño del archivo debe ser válido.";

    public const string IdDisplayName = "Id";
    public const string FileDisplayName = "Portada";
    public const string FileNameDisplayName = "Nombre";
    public const string MimeDisplayName = "MIME";
    public const string SizeDisplayName = "Tamaño";
    public const string StorageDisplayName = "Repositorio";

    public static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    public static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];
}