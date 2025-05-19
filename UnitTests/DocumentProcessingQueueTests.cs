using Infrastructure;
using System.Threading;

namespace UnitTests
{
    /// <summary>
    /// Pruebas unitarias para DocumentProcessingQueue, que verifica el comportamiento FIFO y el bloqueo de la cola.
    /// </summary>
    public class DocumentProcessingQueueTests
    {
        /// <summary>
        /// Verifica que los elementos se desencolan en orden FIFO (primero en entrar, primero en salir).
        /// </summary>
        [Fact]
        public async Task DequeueAsync_ReturnsItemsInFifoOrder()
        {
            // Arrange: Creamos la cola y encolamos tres elementos en orden
            var queue = new DocumentProcessingQueue();
            var first = Guid.NewGuid();
            var second = Guid.NewGuid();
            var third = Guid.NewGuid();
            queue.Enqueue(first);
            queue.Enqueue(second);
            queue.Enqueue(third);

            // Act: Desencolamos los elementos uno por uno
            var result1 = await queue.DequeueAsync(CancellationToken.None);
            var result2 = await queue.DequeueAsync(CancellationToken.None);
            var result3 = await queue.DequeueAsync(CancellationToken.None);

            // Assert: Verificamos que el orden sea FIFO (primero en entrar, primero en salir)
            Assert.Equal(first, result1);
            Assert.Equal(second, result2);
            Assert.Equal(third, result3);
        }

        [Fact]
        public async Task DequeueAsync_WaitsUntilItemIsEnqueued()
        {
            // Arrange: Creamos la cola y un token de cancelación con timeout
            var queue = new DocumentProcessingQueue();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            // Act: Iniciamos el dequeue (debería esperar porque la cola está vacía)
            var dequeueTask = queue.DequeueAsync(cts.Token);

            await Task.Delay(100); // Damos tiempo para que el dequeueTask quede esperando
            Assert.False(dequeueTask.IsCompleted); // Aún no debería haberse completado

            // Encolamos un elemento, lo que debería desbloquear el dequeue
            var item = Guid.NewGuid();
            queue.Enqueue(item);

            var result = await dequeueTask;
            // Assert: El resultado debe ser el elemento encolado
            Assert.Equal(item, result);
        }
    }
}
