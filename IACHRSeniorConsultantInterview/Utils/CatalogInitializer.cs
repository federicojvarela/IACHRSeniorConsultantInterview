using System.Text.Json;
using System.Threading.Tasks;

namespace WebApi.Utils
{
    /// <summary>
    /// Utilidad para inicializar y validar la existencia del archivo de catálogos.
    /// </summary>
    public static class CatalogInitializer
    {
        /// <summary>
        /// Verifica si el archivo de catálogos existe y, si no, lo crea con datos de ejemplo.
        /// Si el archivo existe pero está corrupto, lo reemplaza por datos válidos.
        /// </summary>
        /// <param name="catalogsDestination">Ruta donde debe existir el archivo de catálogos.</param>
        public static async Task EnsureCatalogsExistAsync(string catalogsDestination)
        {
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
                await File.WriteAllTextAsync(catalogsDestination, json);
                Console.WriteLine("Archivo de catálogos creado exitosamente");
            }
            else
            {
                Console.WriteLine("El archivo de catálogos ya existe");
                // Verificar que el contenido sea válido intentando deserializarlo
                try
                {
                    var content = await File.ReadAllTextAsync(catalogsDestination);
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
                    await File.WriteAllTextAsync(catalogsDestination, json);
                    Console.WriteLine("Archivo de catálogos reemplazado exitosamente");
                }
            }
        }
    }
}
