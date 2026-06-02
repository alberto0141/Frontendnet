using System.Net.Http.Headers;
using System.Net.Http.Json;
using frontendnet.Models;
using frontendnet.Models.Validation;

namespace frontendnet.Services;

public class ArchivosClientService(HttpClient client)
{
    public async Task<List<Archivo>> GetAsync(
        CancellationToken cancellationToken = default
    )
    {
        var archivos = await client.GetFromJsonAsync<List<Archivo>>(
            ApiRoutes.Files,
            cancellationToken
        );

        return archivos ?? [];
    }

    public async Task<Archivo?> GetAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        ValidateId(id, nameof(id));

        return await client.GetFromJsonAsync<Archivo>(
            ApiRoutes.FileDetailById(id),
            cancellationToken
        );
    }

    public async Task PostAsync(
        Upload archivo,
        CancellationToken cancellationToken = default
    )
    {
        ValidateUpload(archivo);

        using var form = CreateMultipartContent(archivo);

        var response = await client.PostAsync(
            ApiRoutes.Files,
            form,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    public async Task PutAsync(
        Upload archivo,
        CancellationToken cancellationToken = default
    )
    {
        ValidateUpload(archivo);

        if (archivo.ArchivoId is null)
        {
            throw new ArgumentException("El archivo debe tener un identificador válido.", nameof(archivo));
        }

        ValidateId(archivo.ArchivoId.Value, nameof(archivo.ArchivoId));

        using var form = CreateMultipartContent(archivo);

        var response = await client.PutAsync(
            ApiRoutes.FileById(archivo.ArchivoId.Value),
            form,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        ValidateId(id, nameof(id));

        var response = await client.DeleteAsync(
            ApiRoutes.FileById(id),
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }

    private static MultipartFormDataContent CreateMultipartContent(Upload archivo)
    {
        var safeFileName = Path.GetFileName(archivo.Portada.FileName);

        var fileContent = new StreamContent(archivo.Portada.OpenReadStream());

        if (!string.IsNullOrWhiteSpace(archivo.Portada.ContentType))
        {
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(
                archivo.Portada.ContentType
            );
        }

        var form = new MultipartFormDataContent();
        form.Add(fileContent, "file", safeFileName);

        return form;
    }

    private static void ValidateUpload(Upload archivo)
    {
        ArgumentNullException.ThrowIfNull(archivo);

        if (archivo.Portada is null || archivo.Portada.Length <= 0)
        {
            throw new ArgumentException("Debe seleccionar un archivo válido.", nameof(archivo));
        }

        if (archivo.Portada.Length > FileValidation.MaxFileSizeBytes)
        {
            throw new ArgumentException("El archivo supera el tamaño permitido.", nameof(archivo));
        }

        var extension = Path.GetExtension(archivo.Portada.FileName).ToLowerInvariant();

        if (!FileValidation.AllowedExtensions.Contains(extension) ||
            !FileValidation.AllowedContentTypes.Contains(archivo.Portada.ContentType))
        {
            throw new ArgumentException("El tipo de archivo no está permitido.", nameof(archivo));
        }

        archivo.Nombre = archivo.Nombre?.Trim();
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                "El identificador debe ser mayor a cero."
            );
        }
    }
}