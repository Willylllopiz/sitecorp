# SiteCorp

SiteCorp es una aplicación experimental en .NET 10 LTS para operaciones de Recursos Humanos.

## Arquitectura

- `src/SiteCorp.Domain`: reglas de negocio y entidades del dominio de Recursos Humanos.
- `src/SiteCorp.Application`: casos de uso, puertos de persistencia y mapeo a contratos.
- `src/SiteCorp.Infrastructure`: Entity Framework Core, SQL Server, `DbContext`, repositorios y seed de desarrollo.
- `src/SiteCorp.Api`: endpoints ASP.NET Core que exponen los casos de uso.
- `src/SiteCorp.Web`: cliente Blazor con interfaz operativa para RR. HH.
- `src/SiteCorp.Shared`: contratos compartidos entre backend y cliente.
- `tests/SiteCorp.Domain.Tests`: pruebas unitarias del dominio.

## Base de datos

El API usa SQL Server mediante Entity Framework Core. La cadena local esta en:

- `src/SiteCorp.Api/appsettings.Development.json`

Valor inicial:

```json
"ConnectionStrings": {
  "SiteCorp": "Server=localhost,1433;Database=SiteCorp;User Id=sa;Password=Your_password123;TrustServerCertificate=True;MultipleActiveResultSets=True"
}
```

Para desarrollo local se incluye `docker-compose.yml` con SQL Server Developer:

```bash
docker compose up -d
```

En Mac Apple Silicon, la imagen oficial corre con `platform: linux/amd64`. Microsoft no da soporte oficial a emulacion para SQL Server en contenedores, pero suele ser util para pruebas locales.

El proyecto usa migraciones formales de EF Core. La herramienta `dotnet-ef` queda declarada como herramienta local del repo:

```bash
dotnet tool restore
dotnet tool run dotnet-ef migrations add InitialAuthSchema --project src/SiteCorp.Infrastructure --startup-project src/SiteCorp.Api --output-dir Data/Migrations
dotnet tool run dotnet-ef database update --project src/SiteCorp.Infrastructure --startup-project src/SiteCorp.Api
```

Cuando el API corre en `Development`, aplica migraciones pendientes y si la base esta vacia siembra datos iniciales de RR. HH.

## Esquemas

- `auth`: autenticacion, autorizacion y sesiones.
- `dbo`: tablas iniciales de RR. HH.; se moveran a un esquema propio cuando definamos ese bounded context.

Tablas iniciales de `auth`:

- `auth.Companies`
- `auth.Users`
- `auth.Roles`
- `auth.UserRoles`
- `auth.Permissions`
- `auth.RolePermissions`
- `auth.RefreshTokens`

## Ejecutar localmente

1. Abrir `SiteCorp.sln` en Rider.
2. Levantar SQL Server con `docker compose up -d`, o ajustar la cadena de conexion a tu SQL Server.
3. Aplicar migraciones desde Rider o terminal.
4. Levantar el API:

```bash
dotnet run --project src/SiteCorp.Api
```

5. Levantar el cliente:

```bash
dotnet run --project src/SiteCorp.Web
```

URLs por defecto:

- API: `http://localhost:5099`
- Web: `http://localhost:5193`

El cliente usa datos del API cuando está disponible y cae a datos locales de ejemplo si el API todavía no está levantado.

## Tests

```bash
dotnet test SiteCorp.sln
```
