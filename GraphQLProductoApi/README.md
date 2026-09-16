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

## 2. Crear el proyecto en Supabase

1. Entra a [supabase.com](https://supabase.com) e inicia sesión (puedes usar tu cuenta de GitHub).
2. Clic en **New Project**. Ponle un nombre (ej. `producto-graphql`), define una
   contraseña para la base de datos (guárdala, la necesitas ya) y elige la región
   más cercana.
3. Espera 1-2 minutos a que se aprovisione el proyecto.
4. Ve a **Project Settings → Database**. Ahí encuentras el host de conexión, con
   este formato: `db.xxxxxxxxxxxx.supabase.co`.

## 3. Configurar la conexión

Edita `appsettings.json` y reemplaza los valores de `ConnectionStrings:DefaultConnection`
con los datos de tu proyecto de Supabase:

```json
"DefaultConnection": "Host=db.TU-PROYECTO.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TU-PASSWORD;SSL Mode=Require;Trust Server Certificate=true;"
```

> Nota: se usa el **Session Pooler** de Supabase (host `aws-0-REGION.pooler.supabase.com`,
> usuario `postgres.TU-PROJECT-REF`) en vez de la conexión directa (`db.xxx.supabase.co`),
> porque la conexión directa es IPv6-only y muchas redes domésticas en Colombia
> solo soportan IPv4. El pooler funciona igual de bien para este proyecto.

> Recomendación: no subas tus credenciales reales a GitHub. Usa `dotnet user-secrets`
> o variables de entorno en tu entrega final, y deja el `appsettings.json` del repo
> con valores de ejemplo (como está aquí).

## 4. Restaurar paquetes y crear la migración

```bash
cd GraphQLProductoApi
dotnet restore
dotnet tool install --global dotnet-ef   # si no lo tienes instalado
dotnet ef migrations add InitialCreate
```

No necesitas correr `dotnet ef database update` manualmente: `Program.cs` llama
`db.Database.Migrate()` al arrancar, así que la tabla `Productos` se crea sola
la primera vez que ejecutes la aplicación.

## 5. Ejecutar

```bash
dotnet run
```

La consola mostrará la URL (por defecto `http://localhost:5080/graphql`).
Al abrirla en el navegador aparece **Banana Cake Pop**, el IDE gráfico que
HotChocolate incluye por defecto para probar consultas GraphQL.

## 6. Operaciones disponibles (CRUD)

**Listar productos**
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

**Obtener un producto**
```graphql
query {
  producto(id: 1) {
    id
    nombre
    precio
  }
}
```

**Crear producto**
```graphql
mutation {
  crearProducto(input: { nombre: "Teclado mecánico", descripcion: "Switches rojos", precio: 250000 }) {
    id
    nombre
  }
}
```

**Actualizar producto**
```graphql
mutation {
  actualizarProducto(id: 1, input: { nombre: "Teclado mecánico RGB", descripcion: "Switches rojos", precio: 270000 }) {
    id
    nombre
    precio
  }
}
```

**Eliminar producto**
```graphql
mutation {
  eliminarProducto(id: 1)
}
```

## 7. Probar con Postman

Postman soporta GraphQL de forma nativa: crea una request nueva, selecciona
tipo **GraphQL**, apunta a `http://localhost:5080/graphql` y pega las
queries/mutations de arriba en el editor.

## 8. Manejo de errores

- Validaciones de negocio (nombre vacío, precio negativo) lanzan `GraphQLException`
  con código `VALIDATION_ERROR`.
- Un producto no encontrado lanza `GraphQLException` con código `PRODUCTO_NOT_FOUND`.
- `GraphQLErrorFilter` intercepta excepciones no controladas (p. ej. de EF Core)
  y evita exponer detalles internos al cliente.
