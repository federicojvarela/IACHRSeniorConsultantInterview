# Proyecto basado en Clean Architecture

## Inspiración y objetivos

El proyecto ha sido diseñado siguiendo los principios del patrón Clean Architecture.

Mi inspiración para implementar Clean Architecture surgió de la necesidad de mejorar la estructura y mantenibilidad del proyecto.
Noté que, a medida que el sistema crecía, se volvía más difícil de entender, escalar y testear.
Quería una arquitectura que separara claramente las responsabilidades y permitiera cambiar tecnologías sin afectar la lógica de negocio.
Además, buscaba una solución que facilitara las pruebas unitarias y me ayudara a mantener un código limpio y desacoplado.
Por eso decidí adoptar Clean Architecture, porque me permite construir una base sólida, flexible y alineada con buenas prácticas modernas.

## Clean Architecture

- Independencia de frameworks
- Testabilidad
- Facilidad de mantenimiento y escalabilidad
- Separación clara de responsabilidades

### Capas principales:
1. **Entities (Entidad/Dominio)**: lógica de negocio pura, sin dependencias externas.
2. **Use Cases / Application**: orquesta flujos de negocio; se comunica con interfaces.
3. **Interface Adapters**: transforma datos entre capas y controla el flujo de entrada/salida (controladores, DTOs).
4. **Frameworks & Drivers (Infrastructure)**: frameworks, acceso a datos, sistemas de archivos, servicios externos.

---

## Análisis por capa

### 1. 📁 Core
Ruta: `/Core/`

**Contenido relevante:**
- `Entities/Document.cs`, `Catalog.cs`, etc.
- `Interfaces/`: `IDocumentRepository`, `ICache`, `ILoggerService`, `IDocumentProcessor`, etc.
- `Enums/ProcessingStatus.cs`
- `Services/`: `DocumentService`, `CatalogService`

**Fortalezas:**
- ✅ Entidades modeladas
- ✅ Interfaces separadas por responsabilidad
- ✅ Servicios desacoplados de detalles técnicos
- ✅ Abstracciones para logging, caché y sistema de archivos

---

### 2. 📁 Infrastructure
Ruta: `/Infrastructure/`

**Contenido relevante:**
- `Storage/FileDocumentStorage.cs`: implementación de `IDocumentStorage`
- `Repositories/`: `CatalogRepository`, `DocumentRepository`
- `Services/`: `FileSystemService.cs`, `MemoryCacheService.cs`, `LoggerServices.cs`
- `DependencyInjection.cs`

**Fortalezas:**
- ✅ Cada implementación corresponde a su interfaz en Core
- ✅ Servicios están desacoplados
- ✅ `FileDocumentStorage` usa `IFileSystemService`
- ✅ Centralización de registros en `DependencyInjection.cs`

---

### 3. 📁 Web/API
Ruta: `/IACHRSeniorConsultantInterview/`

**Contenido relevante:**
- `Controllers/CatalogsController.cs`, `DocumentsController.cs`
- `Program.cs`, `appsettings.json`

**Fortalezas:**
- ✅ Controladores livianos
- ✅ `Program.cs` estructurado
- ✅ Uso de `AddInfrastructure(basePath)`

---

### 4. 📁 UnitTests
Ruta: `/UnitTests/`

**Contenido relevante:**
- `CatalogServiceTests.cs`, `DocumentProcessorServiceTests.cs`, etc.

---

## 🔄 Manejo de caché

- `ICache` definido en Core
- `MemoryCacheService` implementa con `IMemoryCache`
- `FileDocumentStorage` usa caché para evitar lecturas repetidas
- Claves bien organizadas (`document_{id}`)

---


# Refactorización según Clean Architecture y SOLID

## 📁 Clase `FileDocumentStorage`

### 1. Principio de Responsabilidad Única (SRP)

**Análisis:**

Originalmente, `FileDocumentStorage` mezclaba responsabilidades como lectura de archivos, almacenamiento de documentos y gestión de caché.

**Estado actual:**

Se mantuvo como clase única, pero se ha desacoplado usando interfaces:
- `ICache` para la gestión de caché.
- `IFileSystemService` para operaciones de archivo.
- `ILoggerService` para trazabilidad.

Esto permite que la clase sea testeable, flexible y que cada responsabilidad sea inyectada y aislada.

---

### 2. Principio Abierto/Cerrado (OCP)

**Análisis:**

La clase ahora depende exclusivamente de interfaces (`IDocumentStorage`, `ICache`, etc.), lo que permite introducir nuevas implementaciones como `DatabaseDocumentStorage` o `RedisCacheService` sin modificar la lógica de `FileDocumentStorage`.

---

### 3. Principio de Sustitución de Liskov (LSP)

**Análisis:**

Las clases concretas (`MemoryCacheService`, `FileSystemService`, etc.) respetan los contratos definidos por sus interfaces. Se pueden sustituir sin afectar el funcionamiento general de la aplicación.

---

### 4. Principio de Inversión de Dependencias (DIP)

**Análisis:**

`FileDocumentStorage` ya no depende directamente de `File`, `Directory` ni de una clase de caché concreta. Ahora todas sus dependencias (`ICache`, `ILoggerService`, `IFileSystemService`) son inyectadas desde el exterior.

---

## 📁 Clase `CatalogsController`

### 1. Uso de `CatalogService` en lugar de `ICatalogRepository`

**Estado actual:**

`CatalogsController` ahora depende de `CatalogService`, el cual encapsula la lógica de negocio. Este servicio delega al repositorio (`ICatalogRepository`) cuando necesita acceder a los datos.

---

### 2. Principio de Responsabilidad Única (SRP)

**Análisis:**

Cada clase cumple una función clara:
- `CatalogsController` responde a peticiones HTTP.
- `CatalogService` orquesta lógica de negocio.
- `CatalogRepository` accede a los datos de los catálogos.

---

### 3. Uso de DTO (`CatalogDto`)

**Estado actual:**

Se usa `CatalogDto` para encapsular los datos de respuesta. Esto permite desacoplar el dominio de la presentación y adaptar la salida a las necesidades del cliente.

---

## 📁 Interfaces y Servicios

### Interfaces implementadas:

- `IDocumentStorage`: define operaciones de almacenamiento de documentos.
- `ICatalogRepository`: define cómo acceder a catálogos.
- `ICache`: abstracción de la lógica de caché.
- `ILoggerService`: logging desacoplado.
- `IFileSystemService`: operaciones de archivos sin acoplarse a `System.IO`.

### Servicios implementados:

- `MemoryCacheService`: implementa `ICache` usando `IMemoryCache`.
- `LoggerServices`: implementación básica de `ILoggerService`.
- `FileSystemService`: implementa acceso a disco de forma desacoplada.

---

## ✅ Conclusión

El proyecto ha sido alineado con los principios de Clean Architecture. Ahora cuenta con:

- Una separación clara de responsabilidades.
- Inversión de dependencias en todas las capas.
- Interfaces desacopladas que permiten pruebas y cambios futuros.
- Servicios inyectables y reemplazables.

---

# Flujo completo para `GetCatalog(string id)`

### Paso a paso:

1. Llamada HTTP → `/api/catalogs/{id}` → `CatalogsController.GetCatalog(string id)`
2. Se invoca `CatalogService.GetCatalogByIdAsync(id)`
3. Busca el catálogo en la caché (`ICache`)
4. Si no está, accede al repositorio (`CatalogRepository`)
5. Guarda en caché si se encontró
6. Devuelve un DTO al cliente

**Respuesta final:**
```json
{
  "id": "document-types",
  "name": "Tipos de Documento",
  "description": "Catálogo de tipos de documentos soportados por el sistema",
  "itemCount": 4
}
```

---

## Tecnologías y patrones involucrados

| Componente              | Rol                                                   |
|-------------------------|--------------------------------------------------------|
| ASP.NET Core            | Enrutamiento y controladores HTTP                      |
| `CatalogService`        | Orquestación y lógica de negocio                       |
| `ICache` (`MemoryCacheService`) | Optimización de acceso a datos                          |
| `ICatalogRepository`    | Acceso a catálogos desde almacenamiento en JSON        |
| DTOs                    | Separación entre dominio y presentación                |
| Dependency Injection    | Inyección de servicios como `ICache`, `ILogger`, etc.  |

