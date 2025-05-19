using Infrastructure;
using Infrastructure.Workers;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;

namespace UnitTests
{
    public class DocumentProcessingWorkerTests
    {
        [Fact]
        public async Task EnqueuedDocumentIsProcessedAndRepositoryUpdated()
        {
            // Configuramos el contenedor de servicios y la cola de procesamiento
            var services = new ServiceCollection();
            var queue = new DocumentProcessingQueue();
            services.AddSingleton<IDocumentProcessingQueue>(queue);

            // Creamos mocks para el repositorio, el procesador y el logger
            var repoMock = new Mock<IDocumentRepository>();
            var processorMock = new Mock<IDocumentProcessor>();
            var loggerMock = new Mock<ILoggerService>();

            // Registramos los mocks como singletons en el contenedor
            services.AddSingleton(repoMock.Object);
            services.AddSingleton(processorMock.Object);
            services.AddSingleton(loggerMock.Object);

            // Construimos el ServiceProvider y obtenemos el scope factory
            var provider = services.BuildServiceProvider();
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            // Preparamos un documento de prueba y configuramos el mock del repositorio
            var documentId = Guid.NewGuid();
            var document = new Document { Id = documentId, FileName = "file" };

            repoMock.Setup(r => r.GetByIdAsync(documentId)).ReturnsAsync(document);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Document>())).Returns(Task.CompletedTask);

            // Usamos un TaskCompletionSource para saber cuándo se procesó el documento
            var processed = new TaskCompletionSource();
            processorMock.Setup(p => p.ProcessDocument(It.Is<Document>(d => d.Id == documentId)))
                .Returns(Task.CompletedTask)
                .Callback(() => processed.SetResult());

            // Creamos el worker y lo iniciamos
            var worker = new DocumentProcessingWorker(scopeFactory);

            await worker.StartAsync(CancellationToken.None);
            queue.Enqueue(documentId); // Encolamos el documento para ser procesado

            // Esperamos a que el documento sea procesado o hasta 5 segundos
            await Task.WhenAny(processed.Task, Task.Delay(TimeSpan.FromSeconds(5)));
            await worker.StopAsync(CancellationToken.None);

            // Verificamos que el procesador y el repositorio hayan sido llamados correctamente
            processorMock.Verify(p => p.ProcessDocument(It.Is<Document>(d => d.Id == documentId)), Times.Once);
            repoMock.Verify(r => r.UpdateAsync(It.IsAny<Document>()), Times.AtLeastOnce());
        }
    }
}
