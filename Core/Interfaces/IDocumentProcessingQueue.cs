using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    /// <summary>
    /// Abstrae una cola segura para el procesamiento asíncrono de documentos.
    /// </summary>
    public interface IDocumentProcessingQueue
    {
        /// <summary>
        /// Encola el identificador de un documento para su procesamiento.
        /// </summary>
        /// <param name="documentId">Identificador del documento.</param>
        void Enqueue(Guid documentId);

        /// <summary>
        /// Desencola el siguiente identificador de documento de forma asíncrona.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>Identificador del documento a procesar.</returns>
        Task<Guid> DequeueAsync(CancellationToken cancellationToken);
    }
}
