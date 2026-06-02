using System.Net;

namespace frontendnet.Models;

public class ApiClientException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public ApiErrorResponse? ErrorResponse { get; }

    public ApiClientException(HttpStatusCode statusCode, ApiErrorResponse? errorResponse)
        : base(errorResponse?.Mensaje ?? $"Error {(int)statusCode} del servidor.")
    {
        StatusCode = statusCode;
        ErrorResponse = errorResponse;
    }
}
