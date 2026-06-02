using System.Text.Json.Serialization;

namespace frontendnet.Models;

public class ApiPagedResponse<T>
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("data")]
    public List<T> Data { get; set; } = [];
}