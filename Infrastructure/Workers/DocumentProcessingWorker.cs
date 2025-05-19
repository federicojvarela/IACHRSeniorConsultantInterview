using Core.Enums;
using Core.Interfaces;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Workers
{
    /// <summary>
    /// Servicio en segundo plano que procesa documentos de la cola de manera asíncrona.
    /// </summary>
    public class DocumentProcessingWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="DocumentProcessingWorker"/>.
        /// </summary>
        public DocumentProcessingWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var queue = scope.ServiceProvider.GetRequiredService<IDocumentProcessingQueue>();
                    var repository = scope.ServiceProvider.GetRequiredService<IDocumentRepository>();
                    var processor = scope.ServiceProvider.GetRequiredService<IDocumentProcessor>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILoggerService>();

                    var documentId = await queue.DequeueAsync(stoppingToken);
                    await ProcessDocumentAsync(documentId, stoppingToken);
                }
            }
        }

        private async Task ProcessDocumentAsync(Guid documentId, CancellationToken token)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IDocumentRepository>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILoggerService>();

                    logger.LogInformation($"Procesando documento: {documentId}");
                    var document = await repository.GetByIdAsync(documentId);
                    if (document == null)
                    {
                        logger.LogWarning($"Documento con ID {documentId} no encontrado.");
                        return;
                    }

                    document.Status = ProcessingStatus.Processing;
                    await repository.UpdateAsync(document);

                    // Simular procesamiento que toma tiempo
                    await Task.Delay(2000, token);

                    var processor = scope.ServiceProvider.GetRequiredService<IDocumentProcessor>();
                    await processor.ProcessDocument(document);

                    document.Status = ProcessingStatus.Completed;
                    document.ProcessingResult = "Procesamiento completado con éxito";
                    await repository.UpdateAsync(document);
                }
            }
            catch (Exception)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IDocumentRepository>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILoggerService>();

                    logger.LogError($"Error procesando el documento: {documentId}. Ocurrió un error durante el procesamiento.");
                    try
                    {
                        var document = await repository.GetByIdAsync(documentId);
                        if (document != null)
                        {
                            document.Status = ProcessingStatus.Failed;
                            document.ProcessingResult = "Error en el procesamiento.";
                            await repository.UpdateAsync(document);
                        }
                        else
                        {
                            logger.LogWarning($"No se pudo recuperar el documento con ID {documentId} después de un error de procesamiento.");
                        }
                    }
                    catch (Exception innerEx)
                    {
                        logger.LogError($"Error al recuperar el documento {documentId} después de un fallo en el procesamiento: {innerEx.Message}");
                    }
                }
            }
        }
    }
}
