using Core.Entities;
using Core.Interfaces;
using Core.Enums;
namespace Core.Services
{
    public class DocumentProcessorService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentProcessor _documentProcessor;
        private readonly LoggerServices _loggerService;

        public DocumentProcessorService(
            IDocumentRepository documentRepository,
            IDocumentProcessor documentProcessor,
            LoggerServices loggerService)
        {
            _documentRepository = documentRepository;
            _documentProcessor = documentProcessor;
            _loggerService = loggerService;
        }

        public Document UploadDocument(string fileName, string contentType, byte[] content)
        {
            _loggerService.LogInformation($"Subiendo documento: {fileName}");

            // Validar entradas
            if (string.IsNullOrEmpty(fileName))
            {
                _loggerService.LogError("El nombre del archivo no puede ser nulo o vacío.");
                throw new ArgumentException("El nombre del archivo no puede ser nulo o vacío.", nameof(fileName));
            }

            var document = new Document
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                ContentType =  contentType ?? "default/type", 
                Content = content,
                UploadDate = DateTime.UtcNow,
                Status = ProcessingStatus.Pending,
                ProcessingResult = string.Empty
            };

            var savedDocument = _documentRepository.Save(document);

            ProcessDocumentSync(savedDocument.Id);

            return savedDocument;
        }

        private void ProcessDocumentSync(Guid documentId)
        {
            try
            {
                _loggerService.LogInformation($"Procesando documento: {documentId}");
                // Obtener el documento del repositorio
                var document = _documentRepository.GetById(documentId);
                if (document == null)
                {
                    _loggerService.LogWarning($"Documento con ID {documentId} no encontrado.");
                    return; 
                }

                document.Status = ProcessingStatus.Processing;
                _documentRepository.Update(document);

                // Simular procesamiento que toma tiempo
                Thread.Sleep(2000);

                // Procesar el documento
                _documentProcessor.ProcessDocument(document);

                document.Status = ProcessingStatus.Completed;
                document.ProcessingResult = "Procesamiento completado con éxito";
                _documentRepository.Update(document);
            }
            catch (Exception)
            {
                // Registrar un mensaje de error genérico sin exponer los detalles de la excepción
                _loggerService.LogError($"Error procesando el documento: {documentId}. Ocurrió un error durante el procesamiento.");
                
                try
                {
                    var document = _documentRepository.GetById(documentId);
                    if (document != null)
                    {
                        document.Status = ProcessingStatus.Failed;
                        document.ProcessingResult = "Error en el procesamiento."; // Mensaje genérico
                        _documentRepository.Update(document);
                    }
                    else
                    {
                        _loggerService.LogWarning($"No se pudo recuperar el documento con ID {documentId} después de un error de procesamiento.");
                    }
                }
                catch (Exception innerEx)
                {
                    // Registrar el error que ocurrió al intentar recuperar el documento
                    _loggerService.LogError($"Error al recuperar el documento {documentId} después de un fallo en el procesamiento: {innerEx.Message}");
                }
            }
        }

        public Document GetDocument(Guid id)
        {
            return _documentRepository.GetById(id);
        }
    }

    public class CatalogService
    {
        private readonly ICatalogRepository _catalogRepository;

        public CatalogService(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        public async Task<Catalog> GetCatalogAsync(string id) // Cambio a async
        {
            return await _catalogRepository.GetCatalogByIdAsync(id);
        }

        public async Task<List<Catalog>> GetAllCatalogsAsync() // Cambio a async
        {
            return await _catalogRepository.GetAllCatalogsAsync();
        }

        public async Task<CatalogItem> GetCatalogItemAsync(string catalogId, string itemId) // Cambio a async
        {
            return await _catalogRepository.GetCatalogItemAsync(catalogId, itemId);
        }
    }
}
