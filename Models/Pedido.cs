using System.Text.Json.Serialization;

namespace frontendnet.Models;

public class Pedido
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("estado")]
    public string Estado { get; set; } = string.Empty;

    [JsonPropertyName("total")]
    public decimal Total { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreadoEn { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime? ActualizadoEn { get; set; }

    [JsonPropertyName("emailUsuario")]
    public string? EmailUsuario { get; set; }

    [JsonPropertyName("detalles")]
    public List<PedidoDetalle> Detalles { get; set; } = [];
}
