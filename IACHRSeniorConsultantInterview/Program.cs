using Infrastructure;
using Infrastructure.Storage;
using System.Text.Json;
using FluentValidation.AspNetCore;
using FluentValidation;
using Core.Validation;

// Crear el builder de la aplicación web
var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();

// Configuración de FluentValidation
builder.Services.AddFluentValidationAutoValidation(); // Validación automática en los endpoints
builder.Services.AddFluentValidationClientsideAdapters(); // Para MVC y Swagger si usás anotaciones
builder.Services.AddValidatorsFromAssemblyContaining<UploadDocumentRequestValidator>(); // Registro de tus validadores

// Agregar servicios de API y documentación
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine("Configurando aplicación...");

// Configurar ruta de datos
string basePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
Console.WriteLine($"Ruta base de datos: {basePath}");

// Asegurarse de que catalogs.json exista en el directorio de datos
string dataDirectory = Path.Combine(basePath, "data");
Directory.CreateDirectory(dataDirectory);
string catalogsDestination = Path.Combine(dataDirectory, "catalogs.json");
Console.WriteLine($"Ruta de catálogos: {catalogsDestination}");

// Verificar explícitamente si el archivo de catálogos existe
if (!File.Exists(catalogsDestination))
{
    Console.WriteLine($"El archivo de catálogos no existe. Creándolo en: {catalogsDestination}");

    // Catálogos de ejemplo
    var sampleCatalogs = new[]
    {
        new
        {
            id = "document-types",
            name = "Tipos de Documento",
            description = "Catálogo de tipos de documentos soportados por el sistema",
            items = new[]
            {
                new { id = "pdf", name = "PDF", value = "application/pdf" },
                new { id = "docx", name = "Documento de Word", value = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                new { id = "xlsx", name = "Documento de Excel", value = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                new { id = "txt", name = "Archivo de Texto", value = "text/plain" }
            }
        },
        new
        {
            id = "document-statuses",
            name = "Estados de Documento",
            description = "Catálogo de estados posibles de un documento en el sistema",
            items = new[]
            {
                new { id = "pending", name = "Pendiente", value = "0" },
                new { id = "processing", name = "En proceso", value = "1" },
                new { id = "completed", name = "Completado", value = "2" },
                new { id = "failed", name = "Error", value = "3" }
            }
        }
    };

    // Serializar y guardar los catálogos en formato JSON
    var json = JsonSerializer.Serialize(sampleCatalogs, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(catalogsDestination, json);
    Console.WriteLine("Archivo de catálogos creado exitosamente");
}
else
{
    Console.WriteLine("El archivo de catálogos ya existe");
    // Verificar que el contenido sea válido intentando deserializarlo
    try
    {
        var content = File.ReadAllText(catalogsDestination);
        var catalogs = JsonSerializer.Deserialize<object[]>(content);
        Console.WriteLine($"El archivo de catálogos contiene {catalogs?.Length ?? 0} elementos");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al leer el archivo de catálogos: {ex.Message}");
        Console.WriteLine("Se reemplazará el archivo con datos válidos");

        // Datos de ejemplo para reemplazar el archivo corrupto
        var sampleCatalogs = new[]
        {
            new
            {
                id = "document-types",
                name = "Tipos de Documento",
                description = "Catálogo de tipos de documentos soportados por el sistema",
                items = new[]
                {
                    new { id = "pdf", name = "PDF", value = "application/pdf" },
                    new { id = "docx", name = "Documento de Word", value = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
                }
            }
        };

        // Serializar y guardar los nuevos catálogos
        var json = JsonSerializer.Serialize(sampleCatalogs, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(catalogsDestination, json);
        Console.WriteLine("Archivo de catálogos reemplazado exitosamente");
    }
}

// Configuración de servicios adicionales
builder.Services.AddInfrastructure(basePath); // Añadir infraestructura
builder.Services.AddLogging(configure => configure.AddConsole()); // Configurar logging
builder.Services.AddMemoryCache(); // Agregar caché en memoria

// Construir la aplicación
var app = builder.Build();

// Obtener el proveedor de servicios
var serviceProvider = app.Services;

// Inicializar el almacenamiento de documentos
var storage = serviceProvider.GetRequiredService<FileDocumentStorage>();
await storage.InitAsync();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    // Habilitar Swagger en desarrollo
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configurar middleware de la aplicación
app.UseHttpsRedirection(); // Redirección HTTPS
app.UseAuthorization(); // Autorización
app.MapControllers(); // Mapeo de controladores

Console.WriteLine("Aplicación configurada correctamente. Iniciando...");
app.Run(); // Iniciar la aplicación