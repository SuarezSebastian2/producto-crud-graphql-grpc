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


## Cómo levantarla

Necesitas el SDK de .NET 8, un proyecto en Supabase, y un cliente gRPC para
probar (Postman ya soporta gRPC nativamente, o puedes usar grpcurl).

1. Crea un segundo proyecto en Supabase (independiente del de GraphQL), por
   ejemplo `producto-grpc`.
2. Igual que en el proyecto GraphQL: "Connect" → "Direct" → "Session pooler",
   copia los datos y pégalos en `appsettings.json`:

```json
"DefaultConnection": "Host=aws-0-REGION.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.TU-PROJECT-REF;Password=TU-PASSWORD;SSL Mode=Require;Trust Server Certificate=true;"
```

3. Restaura y migra:
dotnet restore
dotnet ef migrations add InitialCreate


4. `dotnet run`. Queda escuchando en `http://localhost:5180` (tiene que ser
   HTTP/2 para que gRPC funcione, ya está configurado así en `Program.cs`).

## El servicio (`producto.proto`)

```protobuf
service ProductoService {
  rpc Create (ProductoInput) returns (ProductoMessage);
  rpc Get (ProductoId) returns (ProductoMessage);
  rpc Update (ProductoUpdateInput) returns (ProductoMessage);
  rpc Delete (ProductoId) returns (google.protobuf.Empty);
  rpc List (google.protobuf.Empty) returns (ProductosList);
}
```

Los 5 métodos cubren el CRUD: `Create`, `Get` y `List` para leer, `Update` y
`Delete` para escribir.

## Probando con Postman

1. Nueva request de tipo gRPC, servidor `localhost:5180`.
2. El proyecto tiene reflexión habilitada (`AddGrpcReflection()`), así que
   Postman detecta el servicio solo y te deja elegir el método de una lista,
   sin tener que importar el `.proto` a mano.
3. Elige el método (`producto.ProductoService/Create`, `/Get`, `/Update`,
   `/Delete`, `/List`) y en el mensaje pon algo como:

```json
{
  "nombre": "Mouse inalámbrico",
  "descripcion": "2.4GHz, batería recargable",
  "precio": 85000
}
```

## Manejo de errores

Cada método devuelve un `RpcException` con el código correspondiente:
`InvalidArgument` si el nombre viene vacío o el precio es negativo,
`NotFound` si el producto no existe, y `Internal` para cualquier error que no
se controle explícitamente (esos quedan registrados con `ILogger`, pero al
cliente no le llega el detalle interno).