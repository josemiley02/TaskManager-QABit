# TaskManager-QABit

## 1. Ejecución del proyecto

### 1.1. Requisitos previos

Para ejecutar la solución se requiere:

-   .NET SDK **10.0.114** o una versión compatible con el proyecto.
-   Docker y Docker Compose.
-   PostgreSQL, si se decide ejecutar la base de datos fuera de Docker.
-   Un entorno de desarrollo compatible con .NET, como Visual Studio
    Code o Visual Studio.

La versión del SDK se encuentra fijada mediante `global.json`. Por ello,
se recomienda instalar la versión indicada antes de ejecutar los
comandos de la solución.

### 1.2. Configuración de la base de datos

La solución utiliza PostgreSQL como sistema gestor de base de datos. En
la raíz del repositorio se encuentra `docker-compose.yml`, que permite
iniciar el contenedor de base de datos.

Desde la raíz del proyecto se ejecuta:

``` bash
docker compose up -d
```

El contenedor utiliza la configuración declarada en
`docker-compose.yml`. Antes de iniciar la aplicación, debe comprobarse
que el contenedor se encuentre activo:

``` bash
docker compose ps
```

La cadena de conexión debe configurarse para el entorno local mediante
User Secrets o mediante el mecanismo de configuración establecido para
el proyecto `TaskManager.Api`. No se recomienda almacenar contraseñas
reales dentro de archivos versionados.

Por ejemplo, desde la raíz del repositorio puede establecerse la cadena
de conexión con:

``` bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<cadena-de-conexion>" --project src/TaskManager.Api
```

El nombre `DefaultConnection` debe coincidir con la clave que consume
`Program.cs` en la API. Si el proyecto utiliza otro nombre, debe
sustituirse en el comando.

### 1.3. Aplicación de las migraciones

Una vez configurada la conexión con PostgreSQL, se deben aplicar las
migraciones de Entity Framework Core para crear o actualizar el esquema
de la base de datos.

Si las migraciones se encuentran en `TaskManager.Infrastructure` y la
API es el proyecto de inicio, el comando es:

``` bash
dotnet ef database update \
  --project src/TaskManager.Infrastructure \
  --startup-project src/TaskManager.Api
```

Este comando debe ejecutarse desde la raíz de la solución. Si la
solución utiliza un nombre de `DbContext` específico o una configuración
distinta de diseño, se deberá adaptar el comando a dicha configuración.

### 1.4. Restauración y compilación

Desde la raíz del repositorio se ejecutan:

``` bash
dotnet restore
dotnet build
```

La restauración descarga las dependencias declaradas por los proyectos y
la compilación permite comprobar que la solución se encuentra
correctamente configurada.

### 1.5. Ejecución de la API

En una terminal, desde la raíz del repositorio, se inicia el proyecto
API:

``` bash
dotnet run --project src/TaskManager.Api
```

La API informa en la consola las direcciones HTTP y HTTPS en las que
queda disponible. La dirección efectiva depende de la configuración de
`Properties/launchSettings.json`.

El proyecto de presentación debe apuntar a una de esas direcciones
mediante su configuración `ApiSettings:BaseUrl`.

### 1.6. Ejecución de la aplicación de presentación

En una segunda terminal se inicia el proyecto Blazor:

``` bash
dotnet run --project src/TaskManager.Presentation
```

La consola muestra la dirección local desde la que se puede abrir la
aplicación en el navegador.

Para que la aplicación de presentación pueda consumir la API, el valor
de `ApiSettings:BaseUrl` en
`src/TaskManager.Presentation/appsettings.json` debe corresponder con la
dirección real de la API e incluir la barra final cuando así lo requiera
la configuración del cliente HTTP.

La API y la aplicación de presentación deben permanecer ejecutándose
simultáneamente.

### 1.7. Ejecución de las pruebas

Si se desea ejecutar las pruebas automatizadas, se utiliza:

``` bash
dotnet test
```

------------------------------------------------------------------------

## 2. Estructura del sistema de archivos

La solución está organizada en proyectos independientes, agrupados
dentro de `src`, además del proyecto de pruebas.

``` text
TaskManager-QABit/
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
│   ├── TaskManager.Api/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Extensions/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   └── TaskManager.Presentation/
│       ├── Components/
│       │   ├── Layout/
│       │   ├── Pages/
│       │   └── Shared/
│       ├── Models/
│       ├── Services/
│       ├── wwwroot/
│       ├── Program.cs
│       └── appsettings.json
│
├── tests/
│   └── TaskManager.Tests/
│
├── TaskManager-QABit.sln
├── Directory.Build.props
├── global.json
├── docker-compose.yml
└── README.md
```

### 2.1. TaskManager.Domain

Contiene el núcleo del dominio de la aplicación. En este proyecto se
definen las entidades y enumeraciones que representan los conceptos
principales del sistema, sin depender de tecnologías de persistencia,
presentación o infraestructura.

### 2.2. TaskManager.Application

Contiene los casos de uso y la lógica de aplicación. Aquí se encuentran
los servicios que coordinan las operaciones, los DTOs utilizados para
intercambiar información, las interfaces que necesita la aplicación y
las reglas de validación.

### 2.3. TaskManager.Infrastructure

Implementa los mecanismos técnicos necesarios para acceder a los
recursos externos. En particular, contiene el contexto de Entity
Framework Core, las configuraciones de persistencia, las migraciones y
las implementaciones concretas de los repositorios.

### 2.4. TaskManager.Api

Expone la funcionalidad de la aplicación mediante una API REST. Sus
controladores reciben las solicitudes HTTP, delegan las operaciones a
los servicios de aplicación y devuelven las respuestas con los códigos
de estado correspondientes.

### 2.5. TaskManager.Presentation

Contiene la interfaz web desarrollada con Blazor. Este proyecto consume
la API mediante solicitudes HTTP y presenta los datos al usuario.
Mantiene sus propios modelos de intercambio y servicios cliente, sin
acceder directamente a la base de datos ni a las implementaciones
internas de la aplicación.

### 2.6. TaskManager.Tests

Agrupa las pruebas automatizadas de la solución. Su finalidad es
verificar el comportamiento de las funcionalidades y reducir el riesgo
de introducir regresiones durante futuras modificaciones.

------------------------------------------------------------------------

## 3. Decisiones de diseño y arquitectura

### 3.1. Arquitectura en capas

Se seleccionó una arquitectura en capas por ser una alternativa sencilla
de implementar, comprender y mantener para el alcance del proyecto.

La separación entre dominio, aplicación, infraestructura, API y
presentación permite asignar responsabilidades concretas a cada parte
del sistema. Esta organización respeta el principio de Responsabilidad
Única, ya que cada capa concentra un conjunto delimitado de tareas y
evita mezclar la lógica de negocio con los detalles de persistencia o
comunicación HTTP.

La elección también permitió mantener una estructura clara sin
incorporar patrones o componentes cuya complejidad no estuviera
justificada por los requisitos.

### 3.2. Un único controlador para los recursos de trabajo

Se decidió centralizar las operaciones relacionadas con los elementos de
trabajo (`WorkItems`) en un único controlador, en lugar de crear un
controlador independiente para cada operación.

Esta decisión responde al tamaño y alcance de la aplicación. Las
operaciones de creación, consulta, actualización y eliminación
pertenecen al mismo recurso, por lo que agruparlas en un controlador
REST resulta suficiente y facilita localizar los endpoints.

La división en múltiples controladores habría añadido estructura sin
aportar una ventaja significativa para el volumen funcional actual.

### 3.3. Ausencia de un repositorio genérico

Se optó por implementar un repositorio específico para los elementos de
trabajo, sin introducir un repositorio genérico.

El proyecto tiene un dominio reducido y trabaja principalmente con una
entidad. Por tanto, un repositorio genérico habría incorporado
abstracciones adicionales sin una necesidad concreta que las
justificara.

El repositorio específico permite definir operaciones de acceso a datos
de acuerdo con las necesidades reales del recurso y mantener el código
explícito y sencillo.

### 3.4. Ausencia del patrón Unit of Work

No se implementó una abstracción adicional de Unit of Work. Para el
alcance de esta aplicación, Entity Framework Core y su `DbContext` ya
proporcionan la unidad de trabajo y el seguimiento de cambios necesarios
para agrupar operaciones y persistirlas mediante `SaveChangesAsync`.

Añadir otra capa que envolviera esa funcionalidad habría incrementado la
complejidad sin aportar un beneficio proporcional al tamaño del
proyecto.

### 3.5. Asignación de tareas mediante una cadena de texto

La persona responsable de una tarea se representa mediante una propiedad
de tipo `string`, en lugar de establecer una relación con una entidad
`User`.

Esta decisión se tomó porque el objetivo inicial de la aplicación
consistía en gestionar tareas y no se había definido un modelo de
usuarios ni un sistema de autenticación. Incorporar una entidad de
usuario habría ampliado el dominio y el alcance funcional más allá de
los requisitos establecidos.

La representación textual permite registrar el nombre de la persona
asignada y realizar búsquedas por responsable. No obstante, no
proporciona identidad única, autenticación ni integridad referencial;
estas capacidades podrían incorporarse al introducir un módulo de
usuarios.

### 3.6. DTOs diferenciados para el detalle y el listado

Se definieron modelos de respuesta diferenciados para representar una
tarea individual y para representar los elementos de un listado:
`WorkItemResponse` y `WorkItemList` (o los nombres equivalentes
utilizados en la implementación).

La respuesta del listado no necesita exponer todas las propiedades de
una tarea. Su finalidad es presentar la información necesaria para
identificar y consultar los elementos de trabajo de forma resumida.

El endpoint de consulta por identificador, en cambio, permite recuperar
la información completa del recurso. Esta separación evita transferir
datos innecesarios en las consultas de listado y mantiene una finalidad
clara para el endpoint `GetById`.

### 3.7. Paginación de los resultados

El endpoint de listado incorpora paginación mediante los parámetros
`page` y `pageSize`.

Se estableció un tamaño de página predeterminado de **10 elementos** y
un límite máximo de **100 elementos por página**. También se valida que
el número de página sea mayor que cero y que el tamaño solicitado se
encuentre dentro del rango permitido.

La respuesta paginada incluye:

-   `Items`: elementos correspondientes a la página solicitada.
-   `Page`: número de página actual.
-   `PageSize`: cantidad de elementos solicitados por página.
-   `TotalCount`: cantidad total de registros que cumplen los filtros.
-   `TotalPages`: cantidad total de páginas disponibles.

La paginación evita recuperar todos los registros en una sola petición,
reduce el volumen de información transferida y permite que la interfaz
presente los resultados progresivamente. El conteo total se calcula
aplicando primero los filtros y antes de ejecutar la consulta paginada.

El listado también permite aplicar filtros, como el estado de la tarea y
la persona responsable. De esta forma, la paginación se realiza sobre el
conjunto de resultados filtrado y no sobre la totalidad de los
registros.

### 3.8. Blazor como tecnología de presentación

Para la interfaz se seleccionó Blazor Web App con modo de interactividad
Interactive Server. Esta alternativa permite desarrollar componentes
interactivos utilizando C# y Razor, manteniendo el lenguaje principal
empleado en el backend.

La presentación consume la API REST mediante `HttpClient`, conservando
la separación entre interfaz y lógica de aplicación. De esta manera, el
frontend no accede directamente a Entity Framework Core ni a la base de
datos.

------------------------------------------------------------------------

## 4. Mejoras que podrían incorporarse con más tiempo

La solución cubre las funcionalidades principales de gestión de tareas.
Sin embargo, existen mejoras que ampliarían sus capacidades y
permitirían utilizarla en un entorno con varios usuarios.

### 4.1. Gestión de usuarios, roles y autenticación

Se incorporaría un módulo de usuarios con registro, inicio de sesión y
administración de perfiles. La autenticación se realizaría mediante JWT
(JSON Web Tokens), permitiendo proteger los endpoints y establecer la
identidad del usuario que realiza cada operación.

También se definirían roles y permisos para controlar qué acciones puede
ejecutar cada perfil dentro del sistema.

### 4.2. Prioridad de las tareas

Se añadiría una propiedad `Priority` a las tareas, con valores como
baja, media, alta y crítica.

Esta propiedad permitiría clasificar los elementos de trabajo, ordenar
los listados según su importancia y facilitar la planificación de las
actividades pendientes.

### 4.3. Filtro de tareas asignadas al usuario autenticado

Una vez incorporado el sistema de usuarios, se habilitaría una opción
para consultar exclusivamente las tareas asignadas al usuario que ha
iniciado sesión.

Este filtro permitiría separar las responsabilidades individuales y
ofrecer una vista personalizada de las actividades pendientes de cada
persona.

### 4.4. Sistema de notificaciones por correo electrónico

Se incorporaría un servicio de notificaciones que enviaría correos en
situaciones relevantes, entre ellas:

-   Cuando se asignara una tarea a un usuario.
-   Cuando faltaran 24 horas para alcanzar la fecha límite de una tarea
    asignada.
-   Cuando se modificara la fecha límite o la asignación de una tarea.

Para evitar que el envío de correos afectara al tiempo de respuesta de
la API, se podría implementar una cola de trabajos en segundo plano. Un
proceso independiente se encargaría de procesar y enviar las
notificaciones.

### 4.5. Historial de cambios y auditoría

Se incorporaría un registro de auditoría para conservar información
sobre las modificaciones realizadas en cada tarea: usuario responsable
del cambio, fecha, operación ejecutada y valores modificados.

Esto permitiría conocer la evolución de una tarea y facilitaría la
trazabilidad de las operaciones.

### 4.6. Eliminación lógica

Se podría sustituir la eliminación física por una eliminación lógica
mediante una propiedad como `IsDeleted` o `DeletedAt`.

Esta estrategia permitiría conservar los registros eliminados y
recuperarlos cuando fuera necesario, además de mantener un historial más
completo de la información.

### 4.7. Mejoras en la experiencia de usuario

La interfaz podría ampliarse con:

-   Búsqueda por título y descripción.
-   Filtros combinables por estado, responsable, prioridad y fecha
    límite.
-   Ordenamiento por fecha de creación, vencimiento y prioridad.
-   Indicadores visuales para tareas vencidas o próximas a vencer.
-   Confirmaciones antes de eliminar registros.
-   Mensajes de éxito y error más descriptivos.
-   Diseño adaptable a dispositivos móviles.

### 4.8. Despliegue y observabilidad

Para un entorno de producción se podría incorporar una configuración de
despliegue mediante Docker para la API, la aplicación Blazor y
PostgreSQL.

También sería conveniente añadir logging estructurado, seguimiento de
errores, comprobaciones de disponibilidad (*health checks*) y métricas
básicas de rendimiento. Estas herramientas facilitarían la supervisión y
el diagnóstico de problemas.

### 4.9. Pruebas adicionales y documentación de la API

Se ampliaría la cobertura con pruebas unitarias para los servicios,
pruebas de integración para los repositorios y endpoints, y pruebas de
extremo a extremo para los flujos principales.

Asimismo, se documentarían los contratos de la API, los códigos de
respuesta, los modelos de entrada y salida y los escenarios de error.

------------------------------------------------------------------------

## 5. Uso de herramientas de inteligencia artificial durante el desarrollo

Durante el desarrollo se utilizó una herramienta de inteligencia
artificial como apoyo para la configuración inicial, la implementación
de funcionalidades y la construcción de la interfaz. Las propuestas
generadas fueron revisadas antes de incorporarse al proyecto.

A continuación, se describen las principales tareas en las que se
utilizó esta asistencia, las indicaciones proporcionadas y las
correcciones realizadas.

### 5.1. Configuración inicial del proyecto

**Solicitud realizada:** se solicitó asistencia para estructurar e
inicializar la solución .NET, definir los proyectos que la compondrían,
establecer sus dependencias, configurar PostgreSQL y preparar el entorno
de desarrollo.

**Resultado:** la propuesta permitió establecer la estructura inicial de
la solución, configurar los proyectos y preparar los elementos
necesarios para comenzar el desarrollo.

**Revisión y correcciones:** no fue necesario realizar correcciones
sobre esta parte. La configuración propuesta funcionó correctamente y se
ajustó a las necesidades del proyecto.

### 5.2. Implementación del endpoint de actualización

**Solicitud realizada:** se solicitó asistencia para implementar el
endpoint de actualización de tareas, tomando como referencia la
estructura y el flujo ya utilizados en el endpoint de creación.

**Resultado:** se obtuvo una implementación coherente con la
arquitectura existente, reutilizando la separación entre controlador,
servicio, repositorio y validación.

**Corrección realizada:** durante la revisión se detectó que el estado
de la tarea se estaba recibiendo como una cadena de texto (`string`).
Esto provocaba que el valor predeterminado `Pending` no se reflejara
correctamente durante la creación de una tarea.

Se corrigió el tratamiento del estado para utilizar el tipo
correspondiente a la enumeración definida en el dominio. De esta forma,
se mantuvo la coherencia entre el modelo de dominio, los DTOs y la
representación del estado en las operaciones de la API.

### 5.3. Implementación de la capa de presentación

**Solicitud realizada:** se solicitó orientación para inicializar el
proyecto de presentación con Blazor, configurar la comunicación HTTP con
la API y desarrollar una primera página que consumiera el endpoint de
listado de tareas.

**Resultado:** se generó la estructura inicial de presentación, la
configuración del cliente HTTP, los modelos necesarios para deserializar
las respuestas y el componente encargado de mostrar el listado.

**Revisión y correcciones:** se revisaron los archivos generados
individualmente. Durante esa revisión se detectó un error relacionado
con los tipos de datos de las fechas: el backend declaraba la propiedad
como `DateTime`, mientras que el frontend la recibía como `DateOnly`.

Se ajustó el tipo utilizado en la presentación para mantener la
compatibilidad con el contrato de la API y permitir la deserialización
correcta de las fechas.

Fuera de esta incidencia, el resto de la implementación de la capa de
presentación funcionó correctamente y no requirió modificaciones
relevantes.

### 5.4. Criterio aplicado al uso de IA

La inteligencia artificial se utilizó como herramienta de apoyo al
desarrollo y no como sustituto de la revisión técnica. Las propuestas se
contrastaron con la estructura real del proyecto y se probaron durante
la implementación.

Las correcciones realizadas reflejan la importancia de comprobar la
compatibilidad entre las capas, especialmente en los contratos de datos,
los tipos utilizados y el comportamiento de los endpoints.

------------------------------------------------------------------------

## 6. Consideraciones finales

`TaskManager-QABit` implementa una solución de gestión de tareas basada
en .NET, PostgreSQL y Blazor, organizada mediante una arquitectura en
capas.

La aplicación separa la lógica de dominio, los casos de uso, la
persistencia, la exposición de servicios REST y la presentación web.
Esta separación facilita la comprensión del sistema y permite ampliar
sus funcionalidades sin introducir dependencias innecesarias entre sus
componentes.

Las decisiones de diseño se tomaron teniendo en cuenta el alcance y los
requisitos definidos. Se priorizó una implementación sencilla,
mantenible y coherente, dejando identificadas posibles mejoras para una
evolución posterior.
