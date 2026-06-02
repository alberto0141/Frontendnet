using System.Text.Json.Serialization;

namespace frontendnet.Models;

public class CrearPedidoRequest
{
    [JsonPropertyName("items")]
    public List<CrearPedidoItemRequest> Items { get; set; } = [];
}
