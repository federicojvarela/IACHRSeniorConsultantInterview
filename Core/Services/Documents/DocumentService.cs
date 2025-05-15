using Core.Entities;
using Core.Interfaces;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Documents
{
    public class DocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentProcessor _documentProcessor;
        private readonly ILoggerService _loggerService;

        public DocumentService(
            IDocumentRepository documentRepository,
            IDocumentProcessor documentProcessor,
            ILoggerService loggerService)
        {
            _documentRepository = documentRepository;
            _documentProcessor = documentProcessor;
            _loggerService = loggerService;
        }

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

            await ProcessDocumentAsync(savedDocument.Id);

            return savedDocument;
        }

        private async Task ProcessDocumentAsync(Guid documentId)
        {
            try
            {
                _loggerService.LogInformation($"Procesando documento: {documentId}");
                // Obtener el documento del repositorio
                var document = await _documentRepository.GetByIdAsync(documentId);
                if (document == null)
                {
                    _loggerService.LogWarning($"Documento con ID {documentId} no encontrado.");
                    return;
                }

                document.Status = ProcessingStatus.Processing;
                await _documentRepository.UpdateAsync(document);

                // Simular procesamiento que toma tiempo
                await Task.Delay(2000);

                // Procesar el documento
                await _documentProcessor.ProcessDocument(document);

                document.Status = ProcessingStatus.Completed;
                document.ProcessingResult = "Procesamiento completado con éxito";
                await _documentRepository.UpdateAsync(document);
            }
            catch (Exception)
            {
                // Registrar un mensaje de error genérico sin exponer los detalles de la excepción
                _loggerService.LogError($"Error procesando el documento: {documentId}. Ocurrió un error durante el procesamiento.");

                try
                {
                    var document = await _documentRepository.GetByIdAsync(documentId);
                    if (document != null)
                    {
                        document.Status = ProcessingStatus.Failed;
                        document.ProcessingResult = "Error en el procesamiento."; // Mensaje genérico
                        await _documentRepository.UpdateAsync(document);
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

        public async Task<Document> GetDocument(Guid id)
        {
            return await _documentRepository.GetByIdAsync(id);
        }
    }
}