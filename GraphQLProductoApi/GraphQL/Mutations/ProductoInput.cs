namespace GraphQLProductoApi.GraphQL.Mutations;

/// <summary>
/// Input type usado para crear y actualizar productos vía GraphQL.
/// </summary>
public record ProductoInput(string Nombre, string Descripcion, decimal Precio);
