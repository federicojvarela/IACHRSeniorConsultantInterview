using Core.Entities;
using Core.Enums;

namespace Core.Interfaces
{
    /// <summary>
    /// Define una interfaz para el procesamiento de documentos
    /// </summary>
    public interface IDocumentProcessor
    {
        /// <summary>
        /// Procesa un documento de forma asíncrona
        /// </summary>
        /// <param name="document">El documento a procesar</param>
        Task ProcessDocument(Document document);

        /// <summary>
        /// Verifica el estado actual del procesamiento de un documento
        /// </summary>
        /// <param name="documentId">Identificador único del documento</param>
        /// <returns>El estado actual del procesamiento del documento</returns>
        Task<ProcessingStatus> CheckStatusAsync(Guid documentId);
    }
}