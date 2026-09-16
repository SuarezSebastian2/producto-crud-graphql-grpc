using HotChocolate;
using GraphQLProductoApi.Models;
using GraphQLProductoApi.Repositories;

namespace GraphQLProductoApi.GraphQL.Queries;

/// <summary>
/// Query type: expone las operaciones de lectura (R de CRUD).
/// </summary>
public class ProductoQueries
{
    /// <summary>
    /// Lista todos los productos.
    /// </summary>
    public async Task<IEnumerable<Producto>> GetProductos([Service] IProductoRepository repo)
        => await repo.GetAllAsync();

    /// <summary>
    /// Obtiene un producto por id. Lanza un error GraphQL si no existe.
    /// </summary>
    public async Task<Producto> GetProducto(int id, [Service] IProductoRepository repo)
    {
        var producto = await repo.GetByIdAsync(id);

        if (producto is null)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"No se encontró el producto con id {id}.")
                    .SetCode("PRODUCTO_NOT_FOUND")
                    .Build());
        }

        return producto;
    }
}
