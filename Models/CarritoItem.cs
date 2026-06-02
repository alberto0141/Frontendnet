namespace frontendnet.Models;

public class CarritoItem
{
    public int ProductoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public int? ArchivoId { get; set; }
    public decimal Subtotal => Precio * Cantidad;
}
