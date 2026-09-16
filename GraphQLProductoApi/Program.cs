using Microsoft.EntityFrameworkCore;
using GraphQLProductoApi.Data;
using GraphQLProductoApi.GraphQL.Errors;
using GraphQLProductoApi.GraphQL.Mutations;
using GraphQLProductoApi.GraphQL.Queries;
using GraphQLProductoApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// --- Base de datos (PostgreSQL / Supabase) ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Capa de acceso a datos ---
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

// --- GraphQL (HotChocolate) ---
builder.Services
    .AddGraphQLServer()
    .AddQueryType<ProductoQueries>()
    .AddMutationType<ProductoMutations>()
    .AddErrorFilter<GraphQLErrorFilter>();

var app = builder.Build();

// Aplica migraciones automáticamente al iniciar (crea la BD/tabla si no existe)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Expone el endpoint GraphQL en /graphql, con el IDE Banana Cake Pop incluido
app.MapGraphQL();

app.Run();
