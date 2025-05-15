using Core.Entities;
using Core.Interfaces;
using Core.Enums;
using System.Threading.Tasks;
using System;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Implementación simple del procesador de documentos
    /// </summary>
    public class SimpleDocumentProcessor : IDocumentProcessor
    {
        private readonly IDocumentRepository _documentRepository;

        /// <summary>
        /// Constructor del procesador simple de documentos
        /// </summary>
        /// <param name="documentRepository">Repositorio de documentos a utilizar</param>
        public SimpleDocumentProcessor(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        /// <summary>
        /// Procesa un documento realizando operaciones básicas según su tipo
        /// </summary>
        /// <param name="document">Documento a procesar</param>
        /// <returns>Tarea asíncrona que representa el procesamiento</returns>
        public async Task ProcessDocument(Document document)
        {
            // Verificar si el documento es nulo
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document), "El documento no puede ser nulo.");
            }

            // Simulación simple de procesamiento con retraso de 3 segundos
            await Task.Delay(3000);

            // Actualizar el estado del documento como completado
            document.Status = ProcessingStatus.Completed;

            // Procesar según el tipo de contenido del documento
            switch (document.ContentType?.ToLower())
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
                    // Marcar como fallido si el tipo no es soportado
                    document.Status = ProcessingStatus.Failed;
                    document.ProcessingResult = "Tipo de documento no soportado";
                    break;
            }

            // Persistir los cambios en el repositorio
            await _documentRepository.UpdateAsync(document);
        }

        /// <summary>
        /// Verifica el estado actual del procesamiento de un documento
        /// </summary>
        /// <param name="documentId">Identificador del documento</param>
        /// <returns>Estado actual del procesamiento</returns>
        public async Task<ProcessingStatus> CheckStatusAsync(Guid documentId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            return document?.Status ?? ProcessingStatus.Failed;
        }
    }
}
