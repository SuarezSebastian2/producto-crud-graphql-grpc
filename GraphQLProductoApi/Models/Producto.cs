namespace GraphQLProductoApi.Models;

/// <summary>
/// Entidad Producto gestionada por el ORM (Entity Framework Core).
/// </summary>
public class Producto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    public decimal Precio { get; set; }
}
