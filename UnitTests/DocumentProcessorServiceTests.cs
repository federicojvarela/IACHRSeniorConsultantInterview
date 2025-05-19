using Microsoft.Extensions.Logging;
using Core.Enums;
using Core.Services.Documents;

namespace UnitTests
{
    /// <summary>
    /// Pruebas unitarias para DocumentService, que verifica la lógica de negocio y la interacción con la cola y el repositorio.
    /// </summary>
    public class DocumentProcessorServiceTests
    {
        /// <summary>
        /// Mock para el repositorio de documentos.
        /// </summary>
        private readonly Mock<IDocumentRepository> _mockDocumentRepository;
        /// <summary>
        /// Mock para la cola de procesamiento.
        /// </summary>
        private readonly Mock<IDocumentProcessingQueue> _mockQueue;
        /// <summary>
        /// Instancia del servicio a testear.
        /// </summary>
        private readonly DocumentService _service;
        /// <summary>
        /// Mock para la caché.
        /// </summary>
        private readonly Mock<ICache> _mockCache;

        /// <summary>
        /// Inicializa los mocks y el servicio para los tests.
        /// </summary>
        public DocumentProcessorServiceTests()
        {
            _mockDocumentRepository = new Mock<IDocumentRepository>();
            _mockQueue = new Mock<IDocumentProcessingQueue>();
            _mockCache = new Mock<ICache>();
            _service = new DocumentService(_mockDocumentRepository.Object, new LoggerServices(Mock.Of<ILogger<LoggerServices>>()), _mockQueue.Object);
        }

        /// <summary>
        /// Verifica que UploadDocumentAsync guarda el documento y lo encola para procesamiento.
        /// </summary>
        [Fact]
        public async Task UploadDocumentShouldSaveAndEnqueueDocument()
        {
            var fileName = "test.txt";
            var contentType = "text/plain";
            var content = new byte[] { 1, 2, 3 };
            var doc = new Document { Id = Guid.NewGuid(), FileName = fileName, ContentType = contentType, Content = content };
            _mockDocumentRepository.Setup(r => r.SaveAsync(It.IsAny<Document>())).ReturnsAsync(doc);
            _mockQueue.Setup(q => q.Enqueue(doc.Id));

            var result = await _service.UploadDocumentAsync(fileName, contentType, content);

            Assert.Equal(fileName, result.FileName);
            _mockDocumentRepository.Verify(r => r.SaveAsync(It.IsAny<Document>()), Times.Once);
            _mockQueue.Verify(q => q.Enqueue(doc.Id), Times.Once);
        }

        /// <summary>
        /// Verifica que GetDocument retorna el documento correcto desde el repositorio.
        /// </summary>
        [Fact]
        public async Task GetDocumentShouldReturnDocumentFromRepository()
        {
            var docId = Guid.NewGuid();
            var doc = new Document { Id = docId, FileName = "file" };
            _mockDocumentRepository.Setup(r => r.GetByIdAsync(docId)).ReturnsAsync(doc);
            var result = await _service.GetDocument(docId);
            Assert.Equal(doc, result);
        }

        /// <summary>
        /// Verifica que GetDocument retorna null cuando el repositorio no encuentra el documento.
        /// </summary>
        [Fact]
        public async Task GetDocumentShouldReturnNullWhenRepositoryReturnsNull()
        {
            var docId = Guid.NewGuid();
            _mockDocumentRepository.Setup(r => r.GetByIdAsync(docId)).ReturnsAsync((Document)null);
            var result = await _service.GetDocument(docId);
            Assert.Null(result);
        }
    }
}
