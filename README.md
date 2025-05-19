# Reseña de mejoras en el manejo de caché

Decidí mejorar el manejo de caché del sistema porque identifiqué problemas de funcionamiento, acoplamiento innecesario y bajo rendimiento.

Antes, el servicio de caché estaba mal configurado y era difícil de mantener. Lo reescribí para que sea más limpio, reutilizable y fácil de integrar en distintos puntos del sistema.

También actualicé el almacenamiento de documentos para que trabaje de forma más eficiente y completamente asincrónica, lo que permite un mejor rendimiento general.

Incorporé además una nueva caché especializada para el archivo `catalogs.json`, que mantiene los datos en memoria y se actualiza automáticamente si el archivo cambia.

Esto evita lecturas repetidas del disco y mejora significativamente los tiempos de respuesta.

La motivación principal fue lograr un sistema más rápido, ordenado y fácil de escalar.

Separar la lógica de la caché general de la caché específica me permite controlar mejor cada escenario.

Esto también mejora la experiencia de desarrollo y reduce posibles errores en el futuro.



# Proyecto basado en Clean Architecture

## Inspiración y objetivos

El proyecto ha sido diseñado siguiendo los principios del patrón Clean Architecture.

Mi motivación para adoptar Clean Architecture fue mejorar la estructura, la mantenibilidad y la escalabilidad del sistema. A medida que el proyecto creció, se volvió más difícil de entender y testear. Quise una arquitectura que separara las responsabilidades de forma clara, facilitara los cambios tecnológicos sin impactar la lógica de negocio y permitiera pruebas unitarias simples. Esta arquitectura me permite una base sólida y moderna.

## Clean Architecture

- Independencia de frameworks
- Testabilidad
- Facilidad de mantenimiento y escalabilidad
- Separación clara de responsabilidades

### Capas principales:
1. **Entities (Dominio)**: Lógica de negocio pura, sin dependencias.
2. **Use Cases / Application**: Orquestación de flujos de negocio.
3. **Interface Adapters**: Transformación de datos entre capas (DTOs, controladores).
4. **Frameworks & Drivers (Infrastructure)**: Acceso a datos, archivos, frameworks, servicios externos.

---

## Análisis por capa

### 1. 📁 Core
Ruta: `/Core/`

**Contenido relevante:**
- `Entities/Document.cs`, `Catalog.cs`, etc.
- `Interfaces/`: `IDocumentRepository`, `ICache`, `ILoggerService`, etc.
- `Enums/ProcessingStatus.cs`
- `Services/`: `DocumentService`, `CatalogService`

**Fortalezas:**
- Entidades bien modeladas
- Interfaces desacopladas
- Servicios reutilizables y testables
- Abstracciones para caché, logging y sistema de archivos

---

### 2. 📁 Infrastructure
Ruta: `/Infrastructure/`

**Contenido relevante:**
- `Storage/FileDocumentStorage.cs`: almacenamiento de documentos
- `Services/`: `FileSystemService.cs`, `MemoryCacheService.cs`, `FileCatalogCache.cs`, `LoggerServices.cs`
- `Repositories/`: `CatalogRepository`, `DocumentRepository`
- `DependencyInjection.cs`

**Fortalezas:**
- Implementaciones concretas para cada interfaz de Core
- `FileDocumentStorage` ahora usa `ICache` correctamente con métodos async genéricos
- `MemoryCacheService` rediseñado para ser más limpio, seguro y compatible con DI
- Introducción de `FileCatalogCache`, una caché especializada para `catalogs.json`

---

### 3. 📁 Web/API
Ruta: `/IACHRSeniorConsultantInterview/`

**Contenido relevante:**
- `Controllers/CatalogsController.cs`, `CachedCatalogController.cs`, `DocumentsController.cs`
- `Program.cs`

**Fortalezas:**
- Controladores simples y bien enfocados
- Uso de servicios y no de repositorios directamente
- `CachedCatalogController` expone el contenido de `catalogs.json` cacheado y permite forzar su recarga

---

### 4. 📁 UnitTests
Ruta: `/UnitTests/`

**Contenido relevante:**
- `CatalogServiceTests.cs`, `DocumentProcessorServiceTests.cs`, etc.
- `FileDocumentStorageTests.cs`, `FileCatalogCacheTests.cs`, `MemoryCacheServiceTests.cs`

**Fortalezas:**
- Cobertura para lógica de negocio y manejo de caché
- Tests para escenarios reales y casos borde

---

## 🔄 Manejo de caché actualizado

### 🔹 `MemoryCacheService`
- Antes: usaba `ICache<object>`, con errores de compilación y operaciones riesgosas.
- Ahora: implementa `ICache` (no genérica) con métodos genéricos (`GetAsync<T>`, `SetAsync<T>`), lo que mejora seguridad de tipos y compatibilidad con DI.

### 🔹 `FileDocumentStorage`
- Antes: cacheaba usando `object`, con castings inseguros.
- Ahora: accede al caché de forma tipada y completamente asincrónica.
- Se agregaron métodos como `InvalidateCacheAsync` y `InvalidateAllCacheAsync` para mejorar el control.

### 🔹 `FileCatalogCache`
- Nueva caché especializada para `catalogs.json`
- Lee el archivo una vez y guarda los datos en memoria (TTL)
- Usa `FileSystemWatcher` para invalidar automáticamente la caché si el archivo cambia
- Expuesta por el controlador `CachedCatalogController`

---

## Procesamiento asincrónico de documentos

- `DocumentService.UploadDocumentAsync` encola documentos mediante `IDocumentProcessingQueue`.
- `DocumentProcessingWorker` (en `Infrastructure/Workers`) se ejecuta como servicio hospedado y procesa los elementos de la cola.
- El uso de `Channel<T>` permite tener varias instancias del worker, habilitando la escalabilidad horizontal.

Este diseño desacopla el tiempo de respuesta de la API del procesamiento de documentos. La configuración y ejecución del worker se manejan automáticamente al registrar la infraestructura con `AddInfrastructure` en `Program.cs`.