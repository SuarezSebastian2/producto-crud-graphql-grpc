using Microsoft.EntityFrameworkCore;
using GrpcProductoApi.Models;

namespace GrpcProductoApi.Data;

/// <summary>
/// Contexto de EF Core. Se encarga de crear la tabla Productos
/// y de gestionar el acceso a los datos sin necesidad de SQL manual.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("Productos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Nombre)
                  .IsRequired()
                  .HasMaxLength(150);
            entity.Property(p => p.Descripcion)
                  .HasMaxLength(500);
            entity.Property(p => p.Precio)
                  .HasColumnType("numeric(18,2)")
                  .IsRequired();
        });
    }
}
