using System.Text.Json.Serialization;

namespace frontendnet.Models;

public class CrearPedidoItemRequest
{
    [JsonPropertyName("productoId")]
    public int ProductoId { get; set; }

    [JsonPropertyName("cantidad")]
    public int Cantidad { get; set; }
}
