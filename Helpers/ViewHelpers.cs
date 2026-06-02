namespace frontendnet.Helpers;

public static class ViewHelpers
{
    public static string SafeText(string? value, int maxLength = 120)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "No disponible";

        var normalizedValue = value.Trim();
        return normalizedValue.Length > maxLength
            ? $"{normalizedValue[..maxLength]}..."
            : normalizedValue;
    }

    public static string FormatFecha(DateTime fecha) => fecha.ToString("dd/MM/yyyy HH:mm");

    public static string EstadoBadgeClase(string estado) => estado switch
    {
        "PENDIENTE"  => "bg-warning text-dark",
        "EN_PROCESO" => "bg-info text-dark",
        "ENVIADO"    => "bg-primary",
        "ENTREGADO"  => "bg-success",
        "CANCELADO"  => "bg-danger",
        _            => "bg-secondary"
    };
}
