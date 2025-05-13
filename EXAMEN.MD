# Refactorización de Clases en el Proyecto

## 📁 Clase `FileDocumentStorage`

### 1. Principio de Responsabilidad Única (SRP)

**Análisis:**

La clase `FileDocumentStorage` gestionaba múltiples responsabilidades:
- Almacenamiento de documentos (cargar, guardar, actualizar, eliminar).
- Caché de documentos en memoria.
- Operaciones de entrada/salida con el sistema de archivos.

**Mejora realizada:**

Se ha dividido la funcionalidad en clases separadas:
- `DocumentCache`: Responsable de la lógica de caché.
- `DocumentRepository`: Encargada de las operaciones de entrada/salida con archivos.

Esto facilita el mantenimiento, la extensión y la prueba de cada componente de manera independiente.

---

### 2. Principio Abierto/Cerrado (OCP)

**Análisis:**

La implementación anterior estaba estrechamente acoplada al sistema de archivos y al mecanismo de caché. Cualquier cambio en el método de almacenamiento requería modificar la clase principal.

**Mejora realizada:**

Se han definido interfaces para abstraer el comportamiento:
- `IDocumentStorage`: Define las operaciones de almacenamiento de documentos.
- `ICache`: Define las operaciones de caché.

Esto permite introducir nuevas implementaciones (por ejemplo, `DatabaseDocumentStorage`) sin modificar el código existente.

---

### 3. Principio de Sustitución de Liskov (LSP)

**Análisis:**

Cuando se implementan interfaces, es fundamental que las nuevas clases respeten el contrato establecido.

**Mejora realizada:**

Todas las implementaciones de `IDocumentStorage` e `ICache` han sido diseñadas para funcionar de forma intercambiable, garantizando la correcta sustitución sin afectar la lógica del programa.

---

### 4. Principio de Segregación de Interfaces (ISP)

**Análisis:**

Si la interfaz de almacenamiento de documentos incluye métodos específicos de archivos que no aplican a otros tipos de almacenamiento, esto puede ser problemático.

**Mejora realizada:**

Se han segmentado las interfaces en componentes más específicos. Por ejemplo, las funcionalidades específicas de archivos no se incluyen en la interfaz `IDocumentStorage`, permitiendo implementaciones más enfocadas y reutilizables.

---

### 5. Principio de Inversión de Dependencias (DIP)

**Análisis:**

La clase original dependía directamente de implementaciones concretas de manejo de archivos y caché.

**Mejora realizada:**

Se ha introducido la **inyección de dependencias** para recibir implementaciones de `IDocumentStorage` e `ICache`. Esto facilita la prueba unitaria, promueve el desacoplamiento y aumenta la flexibilidad para futuros cambios.

---

## 📁 Clase `CatalogsController`

### 1. Introducción de la Interfaz `ICatalogRepository`

**Descripción:**

Se creó la interfaz `ICatalogRepository` para definir los métodos que el servicio de catálogo debe implementar.

---

### 2. Principio de Responsabilidad Única (SRP)

**Descripción:**

Al definir una interfaz específica, se asegura que las clases que la implementan solo se preocupen por los métodos que realmente utilizan, evitando sobrecarga innecesaria.

---

### 3. Implementación de `ICatalogRepository` en `CatalogService`

**Descripción:**

La clase `CatalogService` implementa la interfaz `ICatalogRepository`, permitiendo que el controlador dependa de una **abstracción** y no de una implementación concreta.

---

### 4. Principio de Sustitución de Liskov (LSP)

**Descripción:**

Este principio establece que las clases de alto nivel no deben depender de clases de bajo nivel, sino de **abstracciones**. Al inyectar `ICatalogService` en el controlador, se mejora la flexibilidad y capacidad de prueba del código.

---

### 5. Uso de DTO (`CatalogDto`)

**Descripción:**

Se introdujo la clase `CatalogDto` para encapsular los datos que se devuelven desde el controlador.

**Aplicación del SRP:**

Al separar la lógica de presentación de los datos de negocio:
- `CatalogDto` representa los datos enviados al cliente.
- `CatalogService` maneja la lógica de negocio.

---


