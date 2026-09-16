using GraphQLProductoApi.Models;

namespace GraphQLProductoApi.Repositories;

/// <summary>
/// Contrato de acceso a datos para Producto. Aislar esta capa permite que
/// la capa GraphQL no dependa directamente de EF Core ni de detalles de la BD.
/// </summary>
public interface IProductoRepository
{
    Task<IEnumerable<Producto>> GetAllAsync();
    Task<Producto?> GetByIdAsync(int id);
    Task<Producto> CreateAsync(Producto producto);
    Task<Producto?> UpdateAsync(int id, Producto producto);
    Task<bool> DeleteAsync(int id);
}
