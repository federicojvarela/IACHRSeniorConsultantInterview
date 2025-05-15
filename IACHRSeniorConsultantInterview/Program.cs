using Infrastructure;
using Infrastructure.Storage;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();
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

    // Serializar y guardar
    var json = JsonSerializer.Serialize(sampleCatalogs, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(catalogsDestination, json);
    Console.WriteLine("Archivo de catálogos creado exitosamente");
}
else
{
    Console.WriteLine("El archivo de catálogos ya existe");
    // Verificar que el contenido sea válido
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

        // Datos de ejemplo
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

        // Serializar y guardar
        var json = JsonSerializer.Serialize(sampleCatalogs, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(catalogsDestination, json);
        Console.WriteLine("Archivo de catálogos reemplazado exitosamente");
    }
}

// Añadir infraestructura
builder.Services.AddInfrastructure(basePath);

// Configurar logging
builder.Services.AddLogging(configure => configure.AddConsole());

builder.Services.AddMemoryCache();

var app = builder.Build();

// Construir el proveedor de servicios
var serviceProvider = app.Services; // Usar app.Services para obtener el proveedor de servicios

// Inicializar FileDocumentStorage
var storage = serviceProvider.GetRequiredService<FileDocumentStorage>();
await storage.InitAsync();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("Aplicación configurada correctamente. Iniciando...");
app.Run();