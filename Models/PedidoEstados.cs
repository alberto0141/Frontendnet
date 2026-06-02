namespace frontendnet.Models;

public static class PedidoEstados
{
    public const string Pendiente = "PENDIENTE";
    public const string EnProceso = "EN_PROCESO";
    public const string Enviado = "ENVIADO";
    public const string Entregado = "ENTREGADO";
    public const string Cancelado = "CANCELADO";

    public static readonly IReadOnlyList<string> Todos =
        [Pendiente, EnProceso, Enviado, Entregado, Cancelado];

    public static bool EsValido(string? estado) =>
        !string.IsNullOrWhiteSpace(estado) && Todos.Contains(estado);
}
