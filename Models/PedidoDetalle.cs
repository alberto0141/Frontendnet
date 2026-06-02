using System.Text.Json.Serialization;

namespace frontendnet.Models;

public class PedidoDetalle
{
    [JsonPropertyName("productoId")]
    public int ProductoId { get; set; }

    [JsonPropertyName("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [JsonPropertyName("precioUnitario")]
    public decimal PrecioUnitario { get; set; }

    [JsonPropertyName("cantidad")]
    public int Cantidad { get; set; }

    [JsonPropertyName("subtotal")]
    public decimal Subtotal { get; set; }
}
