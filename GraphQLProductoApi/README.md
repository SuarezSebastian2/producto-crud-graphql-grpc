# GraphQL Producto API

Backend en **ASP.NET Core 8 + HotChocolate (GraphQL) + Entity Framework Core + PostgreSQL (Supabase)**
que implementa un CRUD sobre la entidad `Producto`.

## Estructura

```
GraphQLProductoApi/
├── Models/Producto.cs                 # Entidad de dominio
├── Data/AppDbContext.cs               # DbContext de EF Core (ORM)
├── Repositories/                      # Acceso a datos (interfaz + implementación)
├── GraphQL/
│   ├── Queries/ProductoQueries.cs     # Operaciones de lectura
│   ├── Mutations/ProductoMutations.cs # Operaciones de escritura (crear/actualizar/eliminar)
│   ├── Mutations/ProductoInput.cs     # Input type
│   └── Errors/GraphQLErrorFilter.cs   # Manejo centralizado de errores
└── Program.cs                         # Configuración y arranque
```

## 1. Requisitos

- .NET 8 SDK
- Una cuenta gratuita en [Supabase](https://supabase.com) (PostgreSQL en la nube)

## Cómo levantarla

Necesitas el SDK de .NET 8 y un proyecto en Supabase.

1. Crea un proyecto en supabase.com (gratis). Cuando te pida la contraseña de la
   base de datos, guárdala en ese momento, después no se puede volver a ver.
2. En el proyecto, ve al botón "Connect" → "Direct" → pestaña "Session pooler" y
   copia los datos (host, usuario, base). Usé el pooler y no la conexión directa
   porque la directa solo resuelve por IPv6 y en mi red no me conectaba.
3. En `appsettings.json`, reemplaza los valores de `ConnectionStrings:DefaultConnection`:

```json
"DefaultConnection": "Host=aws-0-REGION.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.TU-PROJECT-REF;Password=TU-PASSWORD;SSL Mode=Require;Trust Server Certificate=true;"
```

4. Restaura y crea la migración:
dotnet restore
dotnet ef migrations add InitialCreate


No hace falta correr `dotnet ef database update`, el `Program.cs` aplica la
migración solo al arrancar.

5. `dotnet run`. Por defecto queda en `http://localhost:5080/graphql`. Si abres esa
   URL en el navegador te aparece Banana Cake Pop, el editor gráfico que trae
   HotChocolate para probar queries sin salir del navegador.

## Las 5 operaciones

**Crear**
```graphql
mutation {
  crearProducto(input: { nombre: "Teclado mecánico", descripcion: "Switches rojos", precio: 250000 }) {
    id
    nombre
  }
}
```

**Listar**
```graphql
query {
  productos {
    id
    nombre
    descripcion
    precio
  }
}
```

**Obtener por id**
```graphql
query {
  producto(id: 1) {
    id
    nombre
    descripcion
    precio
  }
}
```

**Actualizar**
```graphql
mutation {
  actualizarProducto(id: 1, input: { nombre: "Teclado mecánico RGB", descripcion: "Switches rojos", precio: 270000 }) {
    id
    nombre
    precio
  }
}
```

**Eliminar**
```graphql
mutation {
  eliminarProducto(id: 1)
}
```

## Probando con Postman

Postman tiene un tipo de request "GraphQL" nativo — apunta a
`http://localhost:5080/graphql`, pega la query o mutation en el editor, y le das
Send.

## Manejo de errores

Si el nombre viene vacío o el precio es negativo, la mutation lanza una
`GraphQLException` con código `VALIDATION_ERROR`. Si pides un producto que no
existe, lanza `PRODUCTO_NOT_FOUND`. Cualquier otra excepción que se me haya
escapado (por ejemplo algo de la base de datos) la intercepta
`GraphQLErrorFilter` para no mostrar detalles internos al cliente.

