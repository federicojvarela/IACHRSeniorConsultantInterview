using Core.Entities;
using Core.Interfaces;
using Core.Enums;
using System;
using System.Threading.Tasks;

namespace Core.Services.Documents
{
    /// <summary>
    /// Servicio de negocio para la gestión y procesamiento de documentos.
    /// </summary>
    public class DocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly ILoggerService _loggerService;
        private readonly IDocumentProcessingQueue _processingQueue;

        /// <summary>
        /// Inicializa una nueva instancia del servicio de documentos.
        /// </summary>
        /// <param name="documentRepository">Repositorio de documentos.</param>
        /// <param name="documentProcessor">Procesador de documentos.</param>
        /// <param name="loggerService">Servicio de logging.</param>
        public DocumentService(
            IDocumentRepository documentRepository,
            ILoggerService loggerService,
            IDocumentProcessingQueue processingQueue)
        {
            _documentRepository = documentRepository;
            _loggerService = loggerService;
            _processingQueue = processingQueue;
        }

        /// <summary>
        /// Sube un nuevo documento al sistema y dispara su procesamiento.
        /// </summary>
        /// <param name="fileName">Nombre del archivo.</param>
        /// <param name="contentType">Tipo de contenido MIME.</param>
        /// <param name="content">Contenido binario del archivo.</param>
        /// <returns>El documento guardado.</returns>
        public async Task<Document> UploadDocumentAsync(string fileName, string contentType, byte[] content)
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
                ContentType = contentType ?? "default/type",
                Content = content,
                UploadDate = DateTime.UtcNow,
                Status = ProcessingStatus.Pending,
                ProcessingResult = string.Empty
            };

            var savedDocument = await _documentRepository.SaveAsync(document);

            _processingQueue.Enqueue(savedDocument.Id);

            return savedDocument;
        }

        /// <summary>
        /// Obtiene un documento por su identificador único.
        /// </summary>
        /// <param name="id">Identificador único del documento.</param>
        /// <returns>El documento encontrado o null si no existe.</returns>
        public async Task<Document?> GetDocument(Guid id)
        {
            return await _documentRepository.GetByIdAsync(id);
        }
    }
}