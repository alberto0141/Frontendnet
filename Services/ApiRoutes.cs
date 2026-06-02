using Microsoft.AspNetCore.WebUtilities;

namespace frontendnet.Services;

public static class ApiRoutes
{
    public const string AuthLogin = "api/auth";
    public const string AuthRegistro = "api/auth/registro";
    public const string AuthTime = "api/auth/tiempo";
    public const string Roles = "api/roles";

    public const string Users = "api/usuarios";
    public const string Files = "api/archivos";
    public const string AuditLogs = "api/bitacora";

    public const string Categories = "api/categorias";
    public const string Products = "api/productos";

    public const string Pedidos = "api/pedidos";
    public const string MisPedidos = "api/pedidos/mios";

    public static string PedidoById(int id) => $"{Pedidos}/{id}";
    public static string PedidoEstadoById(int id) => $"{PedidoById(id)}/estado";

    public static string UserByEmail(string email)
    {
        return $"{Users}/{Uri.EscapeDataString(email)}";
    }

    public static string FileById(int id)
    {
        return $"{Files}/{id}";
    }

    public static string FileDetailById(int id)
    {
        return $"{Files}/{id}/detalle";
    }

    public static string CategoryById(int id)
    {
        return $"{Categories}/{id}";
    }

    public static string ProductById(int id)
    {
        return $"{Products}/{id}";
    }

    public static string ProductsSearch(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return Products;
        }

        return QueryHelpers.AddQueryString(Products, "s", search.Trim());
    }

    public static string ProductCategory(int productId)
    {
        return $"{ProductById(productId)}/categoria";
    }

    public static string ProductCategoryById(int productId, int categoryId)
    {
        return $"{ProductById(productId)}/categoria/{categoryId}";
    }
}