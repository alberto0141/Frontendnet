using System.Text.Json.Serialization;

namespace frontendnet.Models;

public class ApiErrorResponse
{
    [JsonPropertyName("mensaje")]
    public string? Mensaje { get; set; }

    [JsonPropertyName("details")]
    public List<ApiErrorDetail>? Details { get; set; }
}

public class ApiErrorDetail
{
    [JsonPropertyName("msg")]
    public string? Msg { get; set; }

    [JsonPropertyName("path")]
    public string? Path { get; set; }
}
