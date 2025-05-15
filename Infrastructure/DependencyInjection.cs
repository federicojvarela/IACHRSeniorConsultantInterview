using Core.Interfaces;
using Core.Services.Catalogs;
using Core.Services.Documents;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string basePath)
        {
            // Verificar y crear directorios
            Console.WriteLine($"Configurando infraestructura con basePath: {basePath}");
            Directory.CreateDirectory(basePath);

            string dataDirectory = Path.Combine(basePath, "data");
            Directory.CreateDirectory(dataDirectory);

            string catalogsFilePath = Path.Combine(dataDirectory, "catalogs.json");
            Console.WriteLine($"Ruta de catálogos: {catalogsFilePath}");

            // Configuración de almacenamiento
            services.AddSingleton<ICache, MemoryCacheService>();
            services.AddSingleton<ILoggerService, LoggerServices>();
services.AddSingleton<FileDocumentStorage>(provider =>
    new FileDocumentStorage(
        provider.GetRequiredService<ICache>(),
        provider.GetRequiredService<ILoggerService>(),
        provider.GetRequiredService<IFileSystemService>(),
        Path.Combine(basePath, "data")
    )
);


            // Repositorios
            services.AddScoped<IDocumentRepository, DocumentRepository>();

            // Usamos AddSingleton para CatalogRepository y verificamos explícitamente que el archivo de catálogos existe
            var catalogRepository = new CatalogRepository(catalogsFilePath);
            services.AddSingleton<ICatalogRepository>(catalogRepository);

            // Procesadores
            services.AddScoped<IDocumentProcessor, SimpleDocumentProcessor>();

            // Servicios
            services.AddScoped<DocumentService>();
            services.AddScoped<CatalogService>();

            Console.WriteLine("Infraestructura configurada correctamente");
            return services;
        }
    }
}
