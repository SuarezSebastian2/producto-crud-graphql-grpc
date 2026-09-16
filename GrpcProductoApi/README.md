# gRPC Producto API

Backend en **ASP.NET Core 8 + gRPC + Entity Framework Core + PostgreSQL (Supabase)**
que implementa un CRUD sobre la entidad `Producto`.

## Estructura

```
GrpcProductoApi/
├── Protos/producto.proto              # Definición del servicio y mensajes (IDL)
├── Models/Producto.cs                 # Entidad de dominio
├── Data/AppDbContext.cs               # DbContext de EF Core (ORM)
├── Repositories/                      # Acceso a datos (interfaz + implementación)
├── Services/ProductoGrpcService.cs    # Implementación del servicio gRPC (CRUD)
└── Program.cs                         # Configuración y arranque
```

## 1. Requisitos

- .NET 8 SDK
- Una cuenta gratuita en [Supabase](https://supabase.com) (PostgreSQL en la nube)
- Cliente gRPC para pruebas: Postman (soporta gRPC nativamente) o `grpcurl`

## 2. Crear el proyecto en Supabase

Crea un **segundo proyecto de Supabase** (independiente del de GraphQL), por
ejemplo llamado `producto-grpc`, siguiendo los mismos pasos: New Project → nombre
→ contraseña de base de datos → región. Así cada API tiene su propia base,
igual que pedía la actividad con Azure SQL. Ve a **Project Settings → Database**
para obtener el host de conexión (`db.xxxxxxxxxxxx.supabase.co`).

## 3. Configurar la conexión

Edita `appsettings.json` con los datos de este segundo proyecto de Supabase:

```json
"DefaultConnection": "Host=db.TU-PROYECTO.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TU-PASSWORD;SSL Mode=Require;Trust Server Certificate=true;"
```

> Nota: se usa el **Session Pooler** de Supabase (host `aws-0-REGION.pooler.supabase.com`,
> usuario `postgres.TU-PROJECT-REF`) en vez de la conexión directa, porque la
> conexión directa es IPv6-only y muchas redes en Colombia solo soportan IPv4.

## 4. Restaurar paquetes y crear la migración

```bash
cd GrpcProductoApi
dotnet restore
dotnet ef migrations add InitialCreate
```

Al igual que en el proyecto GraphQL, `Program.cs` aplica la migración
automáticamente (`db.Database.Migrate()`) al arrancar.

## 5. Ejecutar

```bash
dotnet run
```

Por defecto queda escuchando en `http://localhost:5180` (HTTP/2, requerido por gRPC).

## 6. Servicio definido (`producto.proto`)

```protobuf
service ProductoService {
  rpc Create (ProductoInput) returns (ProductoMessage);
  rpc Get (ProductoId) returns (ProductoMessage);
  rpc Update (ProductoUpdateInput) returns (ProductoMessage);
  rpc Delete (ProductoId) returns (google.protobuf.Empty);
  rpc List (google.protobuf.Empty) returns (ProductosList);
}
```

Cubre las 4 operaciones CRUD (`Create`, `Get`/`List`, `Update`, `Delete`).

## 7. Probar con Postman

1. Nueva request → **gRPC Request**.
2. URL del servidor: `localhost:5180`.
3. Como el proyecto tiene `AddGrpcReflection()` habilitado en desarrollo,
   Postman puede descubrir automáticamente el servicio y sus métodos
   ("Use server reflection"). Si prefieres, también puedes importar
   directamente `Protos/producto.proto`.
4. Selecciona el método (`producto.ProductoService/Create`, `/Get`, `/Update`,
   `/Delete`, `/List`) e invócalo con el mensaje en formato JSON, por ejemplo:

```json
{
  "nombre": "Mouse inalámbrico",
  "descripcion": "2.4GHz, batería recargable",
  "precio": 85000
}
```

También puedes usar `grpcurl` desde la terminal:

```bash
grpcurl -plaintext -d '{"nombre":"Mouse","descripcion":"Inalámbrico","precio":85000}' \
  localhost:5180 producto.ProductoService/Create
```

## 8. Manejo de errores

Cada método usa `RpcException` con los códigos de estado propios de gRPC:

- `InvalidArgument`: nombre vacío o precio negativo.
- `NotFound`: producto inexistente en `Get`, `Update` o `Delete`.
- `Internal`: errores no controlados (se registran con `ILogger` y no se
  exponen detalles internos al cliente).
