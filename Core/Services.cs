using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class DocumentProcessorService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentProcessor _documentProcessor;

        public DocumentProcessorService(
            IDocumentRepository documentRepository,
            IDocumentProcessor documentProcessor)
        {
            _documentRepository = documentRepository;
            _documentProcessor = documentProcessor;
        }

        public Document UploadDocument(string fileName, string contentType, byte[] content)
        {
            var document = new Document
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                ContentType = contentType,
                Content = content,
                UploadDate = DateTime.UtcNow,
                Status = ProcessingStatus.Pending,
                ProcessingResult = null
            };

            var savedDocument = _documentRepository.Save(document);

            ProcessDocumentSync(savedDocument.Id);

            return savedDocument;
        }

        private void ProcessDocumentSync(Guid documentId)
        {
            try
            {
                // Obtener el documento del repositorio
                var document = _documentRepository.GetById(documentId);
                if (document == null)
                {
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
            catch (Exception ex)
            {
                try
                {
                    var document = _documentRepository.GetById(documentId);
                    if (document != null)
                    {
                        document.Status = ProcessingStatus.Failed;
                        document.ProcessingResult = $"Error en el procesamiento: {ex.Message}";
                        _documentRepository.Update(document);
                    }
                }
                catch
                {
                    // Logger
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
