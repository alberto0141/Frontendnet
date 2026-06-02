using System.Text.Json.Serialization;

namespace frontendnet.Models;

public class CambiarEstadoPedidoRequest
{
    [JsonPropertyName("estado")]
    public string Estado { get; set; } = string.Empty;
}
