Perfecto. Vamos a plantearlo como una prueba técnica profesional, pero sin caer en una arquitectura excesivamente compleja.

Mi recomendación es utilizar una **arquitectura por capas (Layered Architecture)** con cuatro proyectos principales y uno de pruebas. Posteriormente añadiremos Blazor como frontend independiente, consumiendo la API REST.

Hay algo importante que quiero cuidar: quien revise tu código debe poder entender rápidamente dónde está cada responsabilidad, cómo fluye una petición y cómo se accede a PostgreSQL.

# 1. Arquitectura que vamos a implementar

La solución tendrá esta estructura:

```text
TaskManager-QABit/
│
├── src/
│   ├── TaskManager.Domain/
│   │   ├── Entities/
│   │   └── Enums/
│   │
│   ├── TaskManager.Application/
│   │   ├── Interfaces/
│   │   ├── DTOs/
│   │   ├── Validators/
│   │   └── Services/
│   │
│   ├── TaskManager.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── Configurations/
│   │   │   └── Migrations/
│   │   └── DependencyInjection.cs
│   │
│   └── TaskManager.Api/
│       ├── Controllers/
│       ├── Middleware/
│       ├── Extensions/
│       ├── Program.cs
│       └── appsettings.json
│
├── tests/
│   └── TaskManager.Tests/
│
├── TaskManager-QABit.sln
├── Directory.Build.props
└── README.md
```

### Responsabilidad de cada capa

| Proyecto | Responsabilidad |
|---|---|
| Domain | Entidades, enumeraciones y reglas fundamentales del dominio. |
| Application | Casos de uso, DTOs, interfaces y validaciones. |
| Infrastructure | PostgreSQL, Entity Framework Core y persistencia. |
| API | Endpoints HTTP, configuración y middleware. |
| Tests | Pruebas unitarias y de integración. |

**Dependencias:**

```text
              TaskManager.Api
                /         \
               /           \
    TaskManager.Application  TaskManager.Infrastructure
               \           /
                \         /
               TaskManager.Domain
```

No vamos a implementar repositorios genéricos ni Unit of Work. Utilizaremos `DbContext` directamente donde corresponda, manteniendo la lógica de negocio fuera de Infrastructure.

### Tecnologías elegidas

- **.NET SDK:** 10.0.114.
- **ASP.NET Core Web API:** Controllers.
- **Entity Framework Core:** ORM.
- **PostgreSQL:** Base de datos.
- **Npgsql:** Proveedor de PostgreSQL.
- **FluentValidation:** Validación de DTOs.
- **Swagger / OpenAPI:** Documentación de la API.
- **xUnit:** Pruebas.
- **Moq:** Simulación de dependencias en pruebas.

Sobre FastEndpoints: no lo utilizaría en esta prueba. Los Controllers tradicionales de ASP.NET Core son suficientes, conocidos y perfectamente profesionales. Añadir FastEndpoints supondría incorporar otra forma de estructurar los endpoints sin aportar una ventaja especialmente importante para este CRUD.

# 2. Comprobar .NET 10.0.114

Como estás trabajando en Ubuntu, ejecuta desde la terminal:

```bash
dotnet --version
dotnet --list-sdks
```

Debes obtener:

```text
10.0.114
```

Si tienes varias versiones instaladas, podemos fijar la versión del SDK mediante un `global.json`.

Desde la raíz del repositorio:

```bash
dotnet new globaljson --sdk-version 10.0.114 --roll-forward latestPatch
```

Esto hará que el repositorio utilice esa versión del SDK.

# 3. Crear la solución y los proyectos

Sitúate en la carpeta que clonaste:

```bash
cd TaskManager-QABit
```

Primero creamos la solución:

```bash
dotnet new sln -n TaskManager-QABit
```

Creamos las carpetas:

```bash
mkdir src
mkdir tests
```

Ahora los proyectos:

**Domain**

```bash
dotnet new classlib -n TaskManager.Domain -o src/TaskManager.Domain -f net10.0
```

**Application**

```bash
dotnet new classlib -n TaskManager.Application -o src/TaskManager.Application -f net10.0
```

**Infrastructure**

```bash
dotnet new classlib -n TaskManager.Infrastructure -o src/TaskManager.Infrastructure -f net10.0
```

**API**

```bash
dotnet new webapi -n TaskManager.Api -o src/TaskManager.Api -f net10.0 --use-controllers
```

**Tests**

```bash
dotnet new xunit -n TaskManager.Tests -o tests/TaskManager.Tests -f net10.0
```

Añadimos los proyectos a la solución:

```bash
dotnet sln add src/TaskManager.Domain/TaskManager.Domain.csproj

dotnet sln add src/TaskManager.Application/TaskManager.Application.csproj

dotnet sln add src/TaskManager.Infrastructure/TaskManager.Infrastructure.csproj

dotnet sln add src/TaskManager.Api/TaskManager.Api.csproj

dotnet sln add tests/TaskManager.Tests/TaskManager.Tests.csproj
```

# 4. Configurar las referencias entre proyectos

Ahora definimos las dependencias.

Application necesita conocer Domain:

```bash
dotnet add src/TaskManager.Application reference src/TaskManager.Domain
```

Infrastructure necesita conocer Domain y Application:

```bash
dotnet add src/TaskManager.Infrastructure reference src/TaskManager.Domain

dotnet add src/TaskManager.Infrastructure reference src/TaskManager.Application
```

API necesita conocer Application e Infrastructure:

```bash
dotnet add src/TaskManager.Api reference src/TaskManager.Application

dotnet add src/TaskManager.Api reference src/TaskManager.Infrastructure
```

Finalmente, las pruebas:

```bash
dotnet add tests/TaskManager.Tests reference src/TaskManager.Domain

dotnet add tests/TaskManager.Tests reference src/TaskManager.Application

dotnet add tests/TaskManager.Tests reference src/TaskManager.Infrastructure

dotnet add tests/TaskManager.Tests reference src/TaskManager.Api
```

# 5. Instalar los paquetes NuGet

Vamos a mantener las dependencias en los proyectos donde realmente se necesitan.

He elegido versiones compatibles con .NET 10. Por ejemplo, Npgsql 10.0.3 y FluentValidation 12.2.0. [NuGet](https://www.nuget.org/packages/NpgSql.EntityFrameworkCore.PostgreSql/?utm_source=chatgpt.com)

## 5.1. Domain

No instalaremos absolutamente nada.

El dominio debe ser independiente de frameworks, ORM y proveedores de bases de datos.

## 5.2. Application

FluentValidation:

```bash
dotnet add src/TaskManager.Application package FluentValidation --version 12.2.0

dotnet add src/TaskManager.Application package FluentValidation.DependencyInjectionExtensions --version 12.2.0
```

Con esto podremos crear validadores y registrarlos automáticamente mediante inyección de dependencias.

La documentación oficial recomienda precisamente el registro de validadores mediante `AddValidatorsFromAssemblyContaining`. [FluentValidation](https://docs.fluentvalidation.net/en/latest/aspnet.html?utm_source=chatgpt.com)

## 5.3. Infrastructure

Entity Framework Core y PostgreSQL:

```bash
dotnet add src/TaskManager.Infrastructure package Microsoft.EntityFrameworkCore --version 10.0.14

dotnet add src/TaskManager.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL --version 10.0.3
```

El proveedor Npgsql 10.0.3 está preparado para EF Core y .NET 10. [NuGet](https://www.nuget.org/packages/NpgSql.EntityFrameworkCore.PostgreSql/?utm_source=chatgpt.com)

## 5.4. API

Instalaremos el paquete de EF Core Design:

```bash
dotnet add src/TaskManager.Api package Microsoft.EntityFrameworkCore.Design --version 10.0.14
```

También utilizaremos OpenAPI. El template de Web API de .NET 10 ya incorpora su infraestructura inicial, así que no necesitamos añadir otro paquete de Swagger por ahora.

## 5.5. Tests

Instalaremos Moq:

```bash
dotnet add tests/TaskManager.Tests package Moq
```

El proyecto xUnit ya incorpora las dependencias necesarias para ejecutar pruebas.

## 5.6. Instalar EF Core CLI

Para poder generar y ejecutar migraciones:

```bash
dotnet tool install --global dotnet-ef --version 10.0.14
```

Si ya lo tienes instalado:

```bash
dotnet tool update --global dotnet-ef --version 10.0.14
```

Comprueba:

```bash
dotnet ef --version
```

# 6. Configuración general del repositorio

Antes de comenzar a programar, vamos a añadir algunos archivos importantes.

## 6.1. Directory.Build.props

En la raíz:

```bash
touch Directory.Build.props
```

Contenido:

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

Así evitamos repetir estas propiedades en cada proyecto.

## 6.2. .gitignore

```bash
dotnet new gitignore
```

Si ya existe, no necesitas regenerarlo.

Comprueba que ignore:

```gitignore
bin/
obj/
.vs/
.vscode/
.idea/

*.user
*.suo

.env
appsettings.Local.json
```

No ignores `appsettings.json`, porque tendrá la configuración general sin secretos.

## 6.3. Limpiar las clases iniciales

Los templates crean clases que no necesitamos.

```bash
rm -f src/TaskManager.Domain/Class1.cs
rm -f src/TaskManager.Application/Class1.cs
rm -f src/TaskManager.Infrastructure/Class1.cs
```

Revisa también el contenido inicial de API y Tests para eliminar cualquier ejemplo que no vayamos a utilizar.

## 6.4. Primer commit

Aquí ya tenemos la estructura inicial y las dependencias instaladas.

```bash
git add .

git commit -m "chore: initialize .NET 10 solution and layered architecture"
```

Es importante que este sea un commit independiente. Después iremos haciendo commits por funcionalidades.

---

# 7. Configuración inicial de Domain

Vamos a comenzar por el corazón de la aplicación.

La entidad principal será `TaskItem`.

¿Por qué no llamarla simplemente `Task`? Porque puede generar confusión con `System.Threading.Tasks.Task`, que utilizaremos constantemente en métodos asíncronos.

Creamos las carpetas:

```bash
mkdir -p src/TaskManager.Domain/Entities

mkdir -p src/TaskManager.Domain/Enums
```

## 7.1. Enumeración TaskStatus

Archivo:

`src/TaskManager.Domain/Enums/TaskStatus.cs`

```csharp
namespace TaskManager.Domain.Enums;

public enum TaskStatus
{
    Pending = 1,
    InProgress = 2,
    Done = 3
}
```

Utilizaremos valores numéricos internamente y posteriormente configuraremos EF Core para almacenar los estados como texto en PostgreSQL.

Esto facilita la lectura de los registros directamente desde la base de datos.

## 7.2. Entidad TaskItem

Archivo:

`src/TaskManager.Domain/Entities/TaskItem.cs`

```csharp
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TaskStatus Status { get; set; } = TaskStatus.Pending;

    public string Responsible { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}
```

He añadido dos propiedades adicionales:

- `CreatedAt`: fecha de creación.
- `UpdatedAt`: última modificación.

No estaban explícitamente en el requerimiento, pero son campos habituales en una entidad de este tipo y aportan información útil.

### Una decisión de diseño

Por ahora no vamos a crear una entidad `User` ni un sistema de autenticación.

El requerimiento únicamente pide un responsable. Podemos representarlo mediante un nombre de texto.

Si posteriormente quisieras incorporar autenticación, roles y asignación de tareas a usuarios registrados, podríamos cambiarlo por una relación.

---

# 8. Configuración inicial de Infrastructure

Ahora nos ocuparemos de PostgreSQL y Entity Framework Core.

Creamos las carpetas:

```bash
mkdir -p src/TaskManager.Infrastructure/Persistence/Configurations

mkdir -p src/TaskManager.Infrastructure/Persistence/Migrations
```

## 8.1. TaskManagerDbContext

Archivo:

`src/TaskManager.Infrastructure/Persistence/TaskManagerDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence;

public class TaskManagerDbContext : DbContext
{
    public TaskManagerDbContext(
        DbContextOptions<TaskManagerDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TaskManagerDbContext).Assembly
        );
    }
}
```

Fíjate en algo: no estamos configurando las propiedades de las entidades directamente en el DbContext.

Para eso utilizaremos `IEntityTypeConfiguration<T>`.

Es una manera bastante limpia de mantener separadas las configuraciones de persistencia.

## 8.2. TaskItemConfiguration

Archivo:

`src/TaskManager.Infrastructure/Persistence/Configurations/TaskItemConfiguration.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Responsible)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.DueDate)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt);

        builder.HasIndex(t => t.Status);

        builder.HasIndex(t => t.Responsible);
    }
}
```

Dos índices importantes:

- `Status`: para filtrar rápidamente por estado.
- `Responsible`: para filtrar por responsable.

Son precisamente los dos filtros que solicita la prueba.

No necesitamos complicarnos con índices adicionales.

# 9. Configurar la conexión a PostgreSQL

Vamos a configurar la conexión mediante `appsettings.json`.

Archivo:

`src/TaskManager.Api/appsettings.json`

Deja una configuración similar a esta:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=taskmanager_db;Username=postgres;Password=postgres"
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "AllowedHosts": "*"
}
```

Para desarrollo puedes tener un `appsettings.Development.json` con la misma configuración o sobrescribirla mediante variables de entorno.

**Importante:** las contraseñas reales no deben subirse al repositorio. Para esta prueba puedes utilizar User Secrets:

```bash
cd src/TaskManager.Api

dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=taskmanager_db;Username=postgres;Password=miguel02"
```

Después regresa a la raíz:

```bash
cd ../..
```

## 9.1. Levantar PostgreSQL con Docker

Te recomiendo utilizar Docker Compose para que cualquier persona que clone el repositorio pueda levantar la base de datos fácilmente.

Crea en la raíz:

`docker-compose.yml`

```yaml
services:
  postgres:
    image: postgres:18
    container_name: taskmanager_postgres
    restart: unless-stopped

    environment:
      POSTGRES_DB: taskmanager_db
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres

    ports:
      - "5432:5432"

    volumes:
      - taskmanager_data:/var/lib/postgresql

    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres -d taskmanager_db"]
      interval: 5s
      timeout: 5s
      retries: 5

volumes:
  taskmanager_data:
```

Para esta configuración estamos utilizando la imagen oficial de PostgreSQL 18.

Levanta el contenedor:

```bash
docker compose up -d
```

Comprueba:

```bash
docker ps
```

Y para comprobar que PostgreSQL responde:

```bash
docker exec -it taskmanager_postgres \
    pg_isready -U postgres -d taskmanager_db
```

En caso de que prefieras instalar PostgreSQL directamente en Ubuntu, también puedes hacerlo. Docker simplemente facilita reproducir el entorno.

# 10. Registrar Infrastructure mediante Dependency Injection

No quiero que nuestro `Program.cs` termine lleno de configuraciones de Entity Framework, conexiones y servicios.

Por eso vamos a crear una extensión.

Archivo:

`src/TaskManager.Infrastructure/DependencyInjection.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<TaskManagerDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
```

Así, desde nuestra API podremos registrar toda la infraestructura con una sola llamada.

# 11. Configurar Application y FluentValidation

Aunque todavía no hemos implementado los casos de uso, podemos dejar preparado el registro de validadores.

Crea:

`src/TaskManager.Application/DependencyInjection.cs`

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly
        );

        return services;
    }
}
```

Posteriormente, cuando creemos los DTOs, tendremos validadores como:

```text
Validators/
    CreateTaskValidator.cs
    UpdateTaskValidator.cs
```

Los validadores se registrarán automáticamente.

Mi recomendación es utilizar **validación manual mediante inyección de `IValidator<T>` en los Controllers**.

¿Por qué?

- Es explícita.
- Permite controlar fácilmente los errores.
- Funciona perfectamente con operaciones asíncronas.
- No dependemos de integraciones automáticas adicionales de MVC.

# 12. Configurar Program.cs

Ahora conectamos todas las piezas.

Archivo:

`src/TaskManager.Api/Program.cs`

Puedes sustituir el contenido inicial por:

```csharp
using FluentValidation;
using TaskManager.Application;
using TaskManager.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi();

// Application
builder.Services.AddApplication();

// Infrastructure
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string was not configured."
    );

builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

// Development configuration
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

Con esto tenemos:

1. Controllers registrados.
2. OpenAPI disponible durante desarrollo.
3. Validadores registrados.
4. PostgreSQL conectado mediante EF Core.
5. Dependency Injection organizada por capas.

Una observación: `MapOpenApi()` genera el documento OpenAPI, pero no una interfaz gráfica Swagger UI. Si queremos una interfaz visual para explorar los endpoints, añadiremos Swagger UI o Scalar posteriormente.

---

# 13. Primera migración de Entity Framework

Llegados aquí, ya tenemos lo necesario para generar nuestra primera migración.

Desde la raíz del repositorio:

```bash
dotnet ef migrations add InitialCreate \
    --project src/TaskManager.Infrastructure \
    --startup-project src/TaskManager.Api \
    --output-dir Persistence/Migrations
```

Esto generará la migración dentro de Infrastructure.

Después:

```bash
dotnet ef database update \
    --project src/TaskManager.Infrastructure \
    --startup-project src/TaskManager.Api
```

Si todo funciona correctamente, PostgreSQL tendrá:

- La tabla `tasks`.
- La tabla `__EFMigrationsHistory`.
- Los índices de estado y responsable.

Puedes comprobarlo:

```bash
docker exec -it taskmanager_postgres \
    psql -U postgres -d taskmanager_db
```

Y dentro de PostgreSQL:

```sql
\dt
```

Para consultar la estructura:

```sql
\d tasks
```

**Una decisión importante:** mantendremos las migraciones dentro de Infrastructure, porque es el proyecto que conoce Entity Framework y la estructura de persistencia.

No pondremos Entity Framework en Domain.

# 14. Compilar y comprobar que todo funciona

Antes de continuar, ejecutamos:

```bash
dotnet restore

dotnet build
```

Si no existen errores, ejecuta:

```bash
dotnet test
```

Todavía no tendremos pruebas funcionales propias, pero comprobaremos que la solución y el proyecto de tests están correctamente configurados.

También podemos arrancar la API:

```bash
dotnet run --project src/TaskManager.Api
```

Comprueba que el documento OpenAPI está disponible en:

```text
https://localhost:<puerto>/openapi/v1.json
```

El puerto exacto lo encontrarás en la salida de `dotnet run`.

# 15. Segundo commit

Una vez comprobado que PostgreSQL, Infrastructure y las migraciones funcionan:

```bash
git add .

git commit -m "feat: configure PostgreSQL persistence with EF Core"
```

Te recomiendo mantener los commits más o menos así durante el desarrollo:

| Commit | Contenido |
|---|---|
| `chore: initialize .NET 10 solution and layered architecture` | Estructura inicial y proyectos. |
| `feat: configure PostgreSQL persistence with EF Core` | DbContext, configuraciones y migraciones. |
| `feat: implement task domain and application layer` | Casos de uso, DTOs y validadores. |
| `feat: implement task CRUD endpoints` | Crear, listar, consultar, editar y eliminar. |
| `feat: add task filtering and pagination` | Filtros y paginación. |
| `feat: implement global exception handling` | Manejo centralizado de errores. |
| `test: add task management unit and integration tests` | Pruebas. |
| `docs: add project README` | Documentación. |

No tienes que crear todos esos commits ahora. Los iremos haciendo conforme terminemos cada funcionalidad.

---

# 16. ¿Qué nos falta implementar en el backend?

Con lo anterior tendremos preparada la infraestructura, pero todavía no hemos construido la funcionalidad de gestión de tareas.

Yo seguiría este orden:

| Fase | Implementación |
|---|---|
| 1 | Domain: entidad y estados. |
| 2 | Application: DTOs y validadores. |
| 3 | Application: interfaces y casos de uso. |
| 4 | Infrastructure: persistencia. |
| 5 | API: Controllers y endpoints. |
| 6 | API: filtros, paginación y respuestas HTTP. |
| 7 | API: excepciones y errores centralizados. |
| 8 | Tests: unitarios e integración. |
| 9 | README y revisión final. |

### Endpoints que tendremos

La API terminará exponiendo algo parecido a esto:

| Método | Endpoint | Descripción |
|---|---|---|
| POST | `/api/tasks` | Crear tarea. |
| GET | `/api/tasks` | Listar tareas. |
| GET | `/api/tasks/{id}` | Consultar tarea. |
| PUT | `/api/tasks/{id}` | Actualizar tarea. |
| DELETE | `/api/tasks/{id}` | Eliminar tarea. |

Y podremos realizar consultas como:

```http
GET /api/tasks?status=Pending&page=1&pageSize=10
```

```http
GET /api/tasks?responsible=Jose&page=1&pageSize=10
```

```http
GET /api/tasks?status=InProgress&responsible=Jose&page=1&pageSize=10
```

La respuesta de listado tendrá metadatos de paginación:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 10,
  "totalCount": 35,
  "totalPages": 4
}
```

---

# 17. Algunas decisiones que quiero mantener durante el desarrollo

Para que la prueba quede bien presentada, hay varias cosas que no quiero que compliquemos innecesariamente:

**1. No utilizaremos CQRS ni MediatR.**

Para cinco operaciones CRUD no necesitamos introducir patrones adicionales.

**2. No utilizaremos repositorios genéricos.**

EF Core ya proporciona `DbSet`, LINQ y `DbContext`. No necesitamos envolverlos sin una razón.

**3. Utilizaremos DTOs.**

Nunca devolveremos directamente las entidades de Domain desde los Controllers.

**4. FluentValidation se encargará de validar las entradas.**

Por ejemplo, título obligatorio, longitud máxima, responsable obligatorio y fecha límite válida.

**5. Los errores tendrán códigos HTTP adecuados.**

- `200 OK`: consulta o actualización correcta.
- `201 Created`: tarea creada.
- `204 No Content`: eliminación correcta.
- `400 Bad Request`: datos inválidos.
- `404 Not Found`: tarea inexistente.
- `500 Internal Server Error`: error inesperado.

**6. Blazor será independiente del backend.**

Cuando terminemos la API, crearemos un proyecto Blazor Web App que consumirá sus endpoints mediante `HttpClient`.

Esto permitirá demostrar que sabes desarrollar tanto APIs REST como aplicaciones web con .NET.

---

## Mi recomendación para el siguiente paso

No avancemos todavía con Blazor. Primero quiero que tengamos un backend sólido y que podamos probarlo completamente desde Swagger o Postman.

El siguiente bloque debería ser **Domain + Application completos**: DTOs de creación y actualización, DTO de respuesta, DTO de paginación, validadores de FluentValidation y los casos de uso necesarios para gestionar las tareas.

Así construiremos funcionalidad real antes de comenzar con los Controllers.

Y una última cosa: guarda este proyecto como una prueba técnica que también puedas enseñar en tu portfolio. La claridad de la arquitectura, los commits y las decisiones justificadas pueden ser tan importantes como conseguir que el CRUD funcione.

Puedes continuar con ese siguiente bloque y te ayudaré a implementarlo manteniendo exactamente esta arquitectura.