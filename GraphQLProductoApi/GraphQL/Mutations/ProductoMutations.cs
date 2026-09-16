using HotChocolate;
using GraphQLProductoApi.Models;
using GraphQLProductoApi.Repositories;

namespace GraphQLProductoApi.GraphQL.Mutations;

/// <summary>
/// Mutation type: expone las operaciones de escritura (C, U, D de CRUD).
/// </summary>
public class ProductoMutations
{
    public async Task<Producto> CrearProducto(ProductoInput input, [Service] IProductoRepository repo)
    {
        ValidarInput(input);

        var producto = new Producto
        {
            Nombre = input.Nombre,
            Descripcion = input.Descripcion,
            Precio = input.Precio
        };

        return await repo.CreateAsync(producto);
    }

    public async Task<Producto> ActualizarProducto(int id, ProductoInput input, [Service] IProductoRepository repo)
    {
        ValidarInput(input);

        var producto = new Producto
        {
            Nombre = input.Nombre,
            Descripcion = input.Descripcion,
            Precio = input.Precio
        };

        var actualizado = await repo.UpdateAsync(id, producto);

        if (actualizado is null)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"No se encontró el producto con id {id}.")
                    .SetCode("PRODUCTO_NOT_FOUND")
                    .Build());
        }

        return actualizado;
    }

    public async Task<bool> EliminarProducto(int id, [Service] IProductoRepository repo)
    {
        var eliminado = await repo.DeleteAsync(id);

        if (!eliminado)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage($"No se encontró el producto con id {id}.")
                    .SetCode("PRODUCTO_NOT_FOUND")
                    .Build());
        }

        return true;
    }

    private static void ValidarInput(ProductoInput input)
    {
        if (string.IsNullOrWhiteSpace(input.Nombre))
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage("El nombre del producto es obligatorio.")
                    .SetCode("VALIDATION_ERROR")
                    .Build());
        }

        if (input.Precio < 0)
        {
            throw new GraphQLException(
                ErrorBuilder.New()
                    .SetMessage("El precio no puede ser negativo.")
                    .SetCode("VALIDATION_ERROR")
                    .Build());
        }
    }
}
