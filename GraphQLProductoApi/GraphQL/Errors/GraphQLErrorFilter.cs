using HotChocolate;
using Microsoft.EntityFrameworkCore;

namespace GraphQLProductoApi.GraphQL.Errors;

/// <summary>
/// Filtro global de errores: evita filtrar detalles internos (p. ej. de EF Core
/// o SQL Server) al cliente y homogeniza los mensajes de error.
/// </summary>
public class GraphQLErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        if (error.Exception is DbUpdateException)
        {
            return error.WithMessage("Ocurrió un error al guardar los datos en la base de datos.");
        }

        if (error.Exception is not null && error.Exception is not GraphQLException)
        {
            return error.WithMessage("Ocurrió un error inesperado procesando la solicitud.");
        }

        return error;
    }
}
