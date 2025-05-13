using Infrastructure;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine("Configurando aplicación...");

// Configurar ruta de datos
string basePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
Console.WriteLine($"Ruta base de datos: {basePath}");

// Ensure catalogs.json exists in the data directory
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
                new { id = "docx", name = "Word Document", value = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                new { id = "xlsx", name = "Excel Document", value = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                new { id = "txt", name = "Text File", value = "text/plain" }
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
                    new { id = "docx", name = "Word Document", value = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
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

// Configure logging
builder.Services.AddLogging(configure => configure.AddConsole());

var app = builder.Build();

// Configure the HTTP request pipeline
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