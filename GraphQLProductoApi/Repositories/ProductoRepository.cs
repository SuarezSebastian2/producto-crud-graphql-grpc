using Microsoft.EntityFrameworkCore;
using GraphQLProductoApi.Data;
using GraphQLProductoApi.Models;

namespace GraphQLProductoApi.Repositories;

public class ProductoRepository : IProductoRepository
{
    private readonly AppDbContext _context;

    public ProductoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Producto>> GetAllAsync()
        => await _context.Productos.AsNoTracking().ToListAsync();

    public async Task<Producto?> GetByIdAsync(int id)
        => await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<Producto> CreateAsync(Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
        return producto;
    }

    public async Task<Producto?> UpdateAsync(int id, Producto producto)
    {
        var existente = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
        if (existente is null)
        {
            return null;
        }

        existente.Nombre = producto.Nombre;
        existente.Descripcion = producto.Descripcion;
        existente.Precio = producto.Precio;

        await _context.SaveChangesAsync();
        return existente;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existente = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
        if (existente is null)
        {
            return false;
        }

        _context.Productos.Remove(existente);
        await _context.SaveChangesAsync();
        return true;
    }
}
