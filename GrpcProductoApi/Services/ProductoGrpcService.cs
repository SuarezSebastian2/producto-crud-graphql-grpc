using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcProductoApi.Models;
using GrpcProductoApi.Protos;
using GrpcProductoApi.Repositories;

namespace GrpcProductoApi.Services;

/// <summary>
/// Implementación del servicio gRPC ProductoService definido en producto.proto.
/// Traduce entre los mensajes protobuf y la entidad Producto del dominio,
/// delegando el acceso a datos al repositorio (EF Core).
/// </summary>
public class ProductoGrpcService : ProductoService.ProductoServiceBase
{
    private readonly IProductoRepository _repository;
    private readonly ILogger<ProductoGrpcService> _logger;

    public ProductoGrpcService(IProductoRepository repository, ILogger<ProductoGrpcService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override async Task<ProductoMessage> Create(ProductoInput request, ServerCallContext context)
    {
        ValidarInput(request.Nombre, request.Precio);

        try
        {
            var producto = new Producto
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                Precio = (decimal)request.Precio
            };

            var creado = await _repository.CreateAsync(producto);
            return ToMessage(creado);
        }
        catch (Exception ex) when (ex is not RpcException)
        {
            _logger.LogError(ex, "Error creando producto");
            throw new RpcException(new Status(StatusCode.Internal, "Error interno al crear el producto."));
        }
    }

    public override async Task<ProductoMessage> Get(ProductoId request, ServerCallContext context)
    {
        var producto = await _repository.GetByIdAsync(request.Id);

        if (producto is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"No se encontró el producto con id {request.Id}."));
        }

        return ToMessage(producto);
    }

    public override async Task<ProductoMessage> Update(ProductoUpdateInput request, ServerCallContext context)
    {
        ValidarInput(request.Nombre, request.Precio);

        var producto = new Producto
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Precio = (decimal)request.Precio
        };

        var actualizado = await _repository.UpdateAsync(request.Id, producto);

        if (actualizado is null)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"No se encontró el producto con id {request.Id}."));
        }

        return ToMessage(actualizado);
    }

    public override async Task<Empty> Delete(ProductoId request, ServerCallContext context)
    {
        var eliminado = await _repository.DeleteAsync(request.Id);

        if (!eliminado)
        {
            throw new RpcException(new Status(StatusCode.NotFound,
                $"No se encontró el producto con id {request.Id}."));
        }

        return new Empty();
    }

    public override async Task<ProductosList> List(Empty request, ServerCallContext context)
    {
        var productos = await _repository.GetAllAsync();

        var respuesta = new ProductosList();
        respuesta.Items.AddRange(productos.Select(ToMessage));
        return respuesta;
    }

    private static void ValidarInput(string nombre, double precio)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "El nombre del producto es obligatorio."));
        }

        if (precio < 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "El precio no puede ser negativo."));
        }
    }

    private static ProductoMessage ToMessage(Producto producto) => new()
    {
        Id = producto.Id,
        Nombre = producto.Nombre,
        Descripcion = producto.Descripcion,
        Precio = (double)producto.Precio
    };
}
