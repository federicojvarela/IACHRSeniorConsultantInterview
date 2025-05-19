using Core.DTOs;
using Core.Interfaces;
using Core.Services;
using Infrastructure;
using Infrastructure.Storage;
using Microsoft.Extensions.Caching.Memory;
using WebApi.Utils;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de rutas y archivos
Console.WriteLine("Configurando aplicación...");
string basePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data");
string dataDirectory = Path.Combine(basePath, "data");
Directory.CreateDirectory(dataDirectory);
string catalogsDestination = Path.Combine(dataDirectory, "catalogs.json");
Console.WriteLine($"Ruta base de catálogos: {catalogsDestination}");

// 2. Inicialización de catálogos
await CatalogInitializer.EnsureCatalogsExistAsync(catalogsDestination);

// 3. Servicios generales
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(config => config.AddConsole());

// 4. Infraestructura (custom)
builder.Services.AddInfrastructure(basePath);

// 5. Cache general
builder.Services.AddMemoryCache(); // Necesaria para ambos

// ⬇️ REGISTROS CLAROS Y SEPARADOS ⬇️

// ✅ Cache genérica reutilizable
builder.Services.AddSingleton<ICache, MemoryCacheService>();


// ✅ Cache específica para catalogs.json
builder.Services.AddSingleton<ICatalogCache>(provider =>
{
    var cache = provider.GetRequiredService<IMemoryCache>();
    return new FileCatalogCache(dataDirectory, cache);
});

// 6. Build y configuración de la app
var app = builder.Build();

// 7. Inicialización de servicios
var serviceProvider = app.Services;
var storage = serviceProvider.GetRequiredService<IDocumentStorage>();
if (storage is FileDocumentStorage fileStorage)
{
    await fileStorage.InitAsync();
}

// 8. Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 9. Final
Console.WriteLine("Aplicación configurada correctamente. Iniciando...");
app.Run();
