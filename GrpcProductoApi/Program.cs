using Microsoft.EntityFrameworkCore;
using GrpcProductoApi.Data;
using GrpcProductoApi.Repositories;
using GrpcProductoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// --- gRPC ---
builder.Services.AddGrpc();
// La reflexión permite que clientes como Postman/grpcurl descubran el
// servicio sin necesidad de tener el .proto a mano.
builder.Services.AddGrpcReflection();

// --- Base de datos (PostgreSQL / Supabase) ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Capa de acceso a datos ---
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

var app = builder.Build();

// Aplica migraciones automáticamente al iniciar (crea la BD/tabla si no existe)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGrpcService<ProductoGrpcService>();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}

app.MapGet("/", () =>
    "Servidor gRPC ProductoService activo. Usa un cliente gRPC (Postman, grpcurl, BloomRPC) " +
    "apuntando a producto.ProductoService para probar los métodos CRUD.");

app.Run();
