using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Infrastructure
{
    /// <summary>
    /// Implementación en memoria de <see cref="IDocumentProcessingQueue"/> basada en <see cref="Channel{T}"/>.
    /// </summary>
    public class DocumentProcessingQueue : IDocumentProcessingQueue
    {
        private readonly Channel<Guid> _queue = Channel.CreateUnbounded<Guid>();

        /// <inheritdoc />
        public void Enqueue(Guid documentId)
        {
            if (!_queue.Writer.TryWrite(documentId))
            {
                throw new InvalidOperationException("No se pudo encolar el documento.");
            }
        }

        /// <inheritdoc />
        public async Task<Guid> DequeueAsync(CancellationToken cancellationToken)
        {
            var id = await _queue.Reader.ReadAsync(cancellationToken);
            return id;
        }
    }
}
