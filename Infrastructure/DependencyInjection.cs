using Core.Interfaces;
using Core.Services.Documents;
using Infrastructure.Storage;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Services;

namespace Infrastructure
{
    /// <summary>
    /// Clase estática para configurar la inyección de dependencias de la capa de infraestructura.
    /// Proporciona métodos para registrar servicios, repositorios y utilidades en el contenedor DI.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Configura los servicios de infraestructura en el contenedor de dependencias.
        /// Incluye almacenamiento, logging, sistema de archivos, repositorios y servicios de negocio.
        /// </summary>
        /// <param name="services">Colección de servicios a configurar.</param>
        /// <param name="basePath">Ruta base para almacenamiento de archivos.</param>
        /// <returns>La colección de servicios configurada.</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string basePath)
        {
            // Verificar y crear directorios necesarios para la aplicación
            Console.WriteLine($"Configurando infraestructura con basePath: {basePath}");
            Directory.CreateDirectory(basePath);

            // Crear directorio de datos donde se almacenarán los archivos
            string dataDirectory = Path.Combine(basePath, "data");
            Directory.CreateDirectory(dataDirectory);

            // Definir ruta del archivo de catálogos
            string catalogsFilePath = Path.Combine(dataDirectory, "catalogs.json");
            Console.WriteLine($"Ruta de catálogos: {catalogsFilePath}");

            // Configuración de servicios de almacenamiento y utilidades
            services.AddSingleton<ICache, MemoryCacheService>();            // Servicio de caché en memoria
            services.AddSingleton<ILoggerService, LoggerServices>();        // Servicio de logging
            services.AddSingleton<IFileSystemService, FileSystemService>(); // Servicio de sistema de archivos

            // Configuración del almacenamiento de documentos
            services.AddSingleton<FileDocumentStorage>(provider =>
                new FileDocumentStorage(
                    provider.GetRequiredService<ICache>(),
                    provider.GetRequiredService<ILoggerService>(),
                    provider.GetRequiredService<IFileSystemService>(),
                    Path.Combine(basePath, "data")
                )
            );

            // Registro de repositorios
            services.AddScoped<IDocumentRepository, DocumentRepository>();

            // Configuración del repositorio de catálogos como singleton
            var catalogRepository = new CatalogRepository(catalogsFilePath);
            services.AddSingleton<ICatalogRepository>(catalogRepository);

            // Registro de procesadores de documentos
            services.AddScoped<IDocumentProcessor, SimpleDocumentProcessor>();

            // Registro de servicios de negocio
            services.AddScoped<DocumentService>();  // Servicio para gestión de documentos

            Console.WriteLine("Infraestructura configurada correctamente");
            return services;
        }
    }
}
