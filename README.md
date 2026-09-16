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
