# Entrega — Backend con servicios GraphQL y gRPC para CRUD sobre base de datos

**Módulo:** Arquitectura de Aplicaciones Web — Unidad 4
**Stack:** .NET 8 (C#) · Entity Framework Core (ORM) · PostgreSQL (Supabase)

Este repositorio contiene **dos proyectos independientes**, cada uno con su propio
backend, que implementan CRUD sobre la entidad `Producto` (id, nombre, descripción, precio):

| Proyecto | Tecnología de comunicación | Carpeta |
|---|---|---|
| API GraphQL | HotChocolate (GraphQL sobre ASP.NET Core) | `GraphQLProductoApi/` |
| API gRPC | gRPC sobre ASP.NET Core | `GrpcProductoApi/` |

Cada carpeta tiene su propio `README.md` con instrucciones detalladas de
configuración, ejecución y pruebas.

## Arquitectura común a ambos proyectos

Para facilitar mantenimiento y escalabilidad (criterio de "Arquitectura de la
solución" en la rúbrica), ambos proyectos siguen la misma organización en capas:

```
Models/         → Entidad de dominio (Producto)
Data/           → DbContext de EF Core (ORM)
Repositories/   → Acceso a datos, aislado detrás de una interfaz
GraphQL/  o  Services/+Protos/   → Capa de comunicación específica de cada tecnología
Program.cs      → Configuración e inyección de dependencias
```

La capa de comunicación (GraphQL o gRPC) nunca usa EF Core directamente: siempre
pasa por `IProductoRepository`. Esto mantiene ambos proyectos desacoplados y
fáciles de extender.

## Pasos generales antes de grabar el video

1. **Crea dos proyectos en [Supabase](https://supabase.com)** (uno para cada API),
   por ejemplo `producto-graphql` y `producto-grpc`. Es gratis, no pide tarjeta
   de crédito y no requiere verificación académica.
2. Reemplaza las cadenas de conexión en cada `appsettings.json` con el host
   `db.TU-PROYECTO.supabase.co` de cada proyecto de Supabase (puerto 5432).
3. En cada proyecto:
   ```bash
   dotnet restore
   dotnet ef migrations add InitialCreate
   dotnet run
   ```
   (la migración se aplica sola al arrancar).
4. Prueba cada operación CRUD con Postman (GraphQL nativo / gRPC nativo — ver
   el README de cada proyecto para el detalle).
5. Sube ambos proyectos a un repositorio de GitHub, cada uno en su carpeta
   (o en dos repos separados, como prefieras), asegurándote de que `.gitignore`
   excluya `bin/` y `obj/`.

## Checklist según la rúbrica

- [ ] **Arquitectura de la solución (GraphQL y gRPC):** carpetas Models/Data/
      Repositories/GraphQL o Services — ya organizado en este repo.
- [ ] **Implementación (GraphQL y gRPC):** CRUD completo + ORM (EF Core) +
      manejo de errores — ya implementado; verifica que compile en tu máquina
      con `dotnet build`.
- [ ] **Pruebas (GraphQL y gRPC):** prueba los 5 métodos de cada API con
      Postman y grábalo en el video (create, get, list, update, delete).
- [ ] **Repositorio:** sube el código (sin `bin/`/`obj/`) a GitHub y comparte
      la URL en el video.
- [ ] **Video (máx. 15 min):** muestra el paso a paso de la construcción de
      cada aplicación, las pruebas con Postman, y la URL del repositorio.

## Nota sobre el tipo de dato `precio`

La lectura fundamental usa `double precio` en el ejemplo de `.proto`. Aquí se
usa `decimal` en la entidad y `numeric(18,2)` en la base de datos (PostgreSQL)
por ser la práctica recomendada para valores monetarios (evita errores de
redondeo de punto flotante), y se convierte a `double` solo en el mensaje gRPC,
tal como exige el tipo del `.proto`. Puedes explicar esta decisión de diseño en
tu video.
