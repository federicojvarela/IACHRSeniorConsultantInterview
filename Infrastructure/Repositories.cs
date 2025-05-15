using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using System.Text.Json;
using Core.Enums;

namespace Infrastructure.Repositories
{
    #region Document Repository
    public class DocumentRepository : IDocumentRepository
    {
        private readonly FileDocumentStorage _storage;

        public DocumentRepository(FileDocumentStorage storage)
        {
            _storage = storage;
        }

        public async Task<List<Document>> GetAllAsync()
        {
            return await _storage.GetAllAsync();
        }

        public async Task<Document> GetByIdAsync(Guid id)
        {
            return await Task.Run(() => _storage.GetByIdAsync(id));
        }

        public async Task<Document> SaveAsync(Document document)
        {
            return await Task.Run(() => _storage.Save(document));
        }

        public async Task UpdateAsync(Document document)
        {
            await Task.Run(() => _storage.Update(document));
        }

        public async Task DeleteAsync(Guid id)
        {
            await Task.Run(() => _storage.Delete(id));
        }
    }
    #endregion

    #region Catalog Repository
    public class CatalogRepository : ICatalogRepository
    {
        private readonly string _catalogFilePath;
        private List<Catalog> _catalogs = new List<Catalog>();

        public CatalogRepository(string catalogFilePath)
        {
            _catalogFilePath = catalogFilePath;
            LoadCatalogs();
        }

        private void LoadCatalogs()
        {
            try
            {
                // Simulamos una carga lenta
                Thread.Sleep(1000);

                // Leer el contenido del archivo
                var json = File.ReadAllText(_catalogFilePath);
                Console.WriteLine($"Contenido leído del archivo: {json}");

                // Opciones de deserialización
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                // Intento de deserialización
                _catalogs = JsonSerializer.Deserialize<List<Catalog>>(json, options) ?? new List<Catalog>();

                // Comprobación detallada del resultado
                if (_catalogs.Count == 0 || _catalogs.Any(c => string.IsNullOrEmpty(c.Id)))
                {
                    throw new InvalidOperationException("El archivo de catálogos está corrupto o los datos no son válidos");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar catálogos: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // En caso de error, inicializamos con catálogos de muestra
                _catalogs = new List<Catalog>
                {
                    new Catalog
                    {
                        Id = "document-types",
                        Name = "Tipos de Documento",
                        Description = "Catálogo de tipos de documentos soportados por el sistema",
                        Items = new List<CatalogItem>
                        {
                            new CatalogItem { Id = "pdf", Name = "PDF", Value = "application/pdf" },
                            new CatalogItem { Id = "docx", Name = "Word Document", Value = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
                        }
                    }
                };

                Console.WriteLine("Se cargaron catálogos de respaldo por defecto");

                // Intentar corregir el archivo
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    var json = JsonSerializer.Serialize(_catalogs, options);
                    File.WriteAllText(_catalogFilePath, json);
                    Console.WriteLine("Archivo de catálogos corregido y guardado");
                }
                catch (Exception writeEx)
                {
                    Console.WriteLine($"No se pudo corregir el archivo: {writeEx.Message}");
                }
            }
        }

        public async Task<Catalog> GetCatalogByIdAsync(string id)
        {
            // Simulamos una consulta lenta
            await Task.Delay(500);
            return _catalogs.FirstOrDefault(c => c.Id == id) ?? new Catalog(); // Cambio para evitar null
        }

        public async Task<List<Catalog>> GetAllCatalogsAsync()
        {
            // Simulamos una consulta lenta
            await Task.Delay(500);
            return _catalogs;
        }

        public async Task<CatalogItem> GetCatalogItemAsync(string catalogId, string itemId)
        {
            // Simulamos una consulta lenta
            await Task.Delay(300);
            var catalog = _catalogs.FirstOrDefault(c => c.Id == catalogId);
            return catalog?.Items.FirstOrDefault(i => i.Id == itemId) ?? new CatalogItem(); // Cambio para evitar null  
        }
    }
    #endregion

    #region DocumentProcessor
    public class SimpleDocumentProcessor : IDocumentProcessor
    {
        private readonly IDocumentRepository _documentRepository;

        public SimpleDocumentProcessor(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task ProcessDocument(Document document)
        {
            // Simulación simple de procesamiento

            // Simular trabajo que toma tiempo
            await Task.Delay(3000);

            // Actualizar el documento con el resultado del procesamiento
            document.Status = ProcessingStatus.Completed;

            switch (document.ContentType.ToLower())
            {
                case "application/pdf":
                    document.ProcessingResult = "PDF procesado correctamente";
                    break;
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    document.ProcessingResult = "Documento Word procesado correctamente";
                    break;
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    document.ProcessingResult = "Hoja de cálculo Excel procesada correctamente";
                    break;
                case "text/plain":
                    document.ProcessingResult = "Archivo de texto procesado correctamente";
                    break;
                default:
                    document.Status = ProcessingStatus.Failed;
                    document.ProcessingResult = "Tipo de documento no soportado";
                    break;
            }

            await _documentRepository.UpdateAsync(document);
        }

        public async Task<ProcessingStatus> CheckStatusAsync(Guid documentId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            return document?.Status ?? ProcessingStatus.Failed;
        }
    }
    #endregion
}
