namespace frontendnet.Models;

public class ResumenCarrito
{
    public List<CarritoItem> Items { get; set; } = [];
    public decimal Total => Items.Sum(i => i.Subtotal);
    public int TotalProductos => Items.Sum(i => i.Cantidad);
}
