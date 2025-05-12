using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using System.Text.Json;

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

        public Document GetById(Guid id)
        {
            return _storage.GetById(id);
        }

        public List<Document> GetAll()
        {
            return _storage.GetAll();
        }

        public Document Save(Document document)
        {
            return _storage.Save(document);
        }

        public void Update(Document document)
        {
            _storage.Update(document);
        }

        public void Delete(Guid id)
        {
            _storage.Delete(id);
        }
    }
    #endregion

    #region Catalog Repository
    public class CatalogRepository : ICatalogRepository
    {
        private readonly string _catalogFilePath;
        private List<Catalog> _catalogs;

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
                _catalogs = JsonSerializer.Deserialize<List<Catalog>>(json, options);

                // Comprobación detallada del resultado
                if (_catalogs == null)
                {
                    Console.WriteLine("La deserialización resultó en NULL");
                    _catalogs = new List<Catalog>();
                }
                else
                {
                    Console.WriteLine($"Se deserializaron {_catalogs.Count} catálogos");
                    foreach (var catalog in _catalogs)
                    {
                        Console.WriteLine($"Catálogo: Id={catalog.Id}, Name={catalog.Name}, Items={catalog.Items?.Count ?? 0}");
                    }
                }

                // Si la deserialización resultó en una lista vacía o con elementos incompletos
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

        public Catalog GetCatalogById(string id)
        {
            // Simulamos una consulta lenta
            Thread.Sleep(500);
            return _catalogs.FirstOrDefault(c => c.Id == id);
        }

        public List<Catalog> GetAllCatalogs()
        {
            // Simulamos una consulta lenta
            Thread.Sleep(500);
            return _catalogs;
        }

        public CatalogItem GetCatalogItem(string catalogId, string itemId)
        {
            // Simulamos una consulta lenta
            Thread.Sleep(300);
            var catalog = _catalogs.FirstOrDefault(c => c.Id == catalogId);
            return catalog?.Items.FirstOrDefault(i => i.Id == itemId);
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

        public void ProcessDocument(Document document)
        {
            // Simulación simple de procesamiento

            // Simular trabajo que toma tiempo
            Thread.Sleep(3000);

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

            _documentRepository.Update(document);
        }

        public ProcessingStatus CheckStatus(Guid documentId)
        {
            var document = _documentRepository.GetById(documentId);
            return document?.Status ?? ProcessingStatus.Failed;
        }
    }
    #endregion
}
