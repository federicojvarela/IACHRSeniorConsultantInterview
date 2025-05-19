using Core.Enums;
using Infrastructure.Repositories;

namespace UnitTests
{
    public class SimpleDocumentProcessorTests
    {
        /// <summary>
        /// Mock para el repositorio de documentos
        /// </summary>
        private readonly Mock<IDocumentRepository> _mockRepo;
        /// <summary>
        /// Procesador a testear
        /// </summary>
        private readonly SimpleDocumentProcessor _processor;

        /// <summary>
        /// Inicializa los mocks y el procesador para los tests
        /// </summary>
        public SimpleDocumentProcessorTests()
        {
            _mockRepo = new Mock<IDocumentRepository>();
            _processor = new SimpleDocumentProcessor(_mockRepo.Object);
        }

        /// <summary>
        /// Verifica que procesar un documento nulo lanza ArgumentNullException
        /// </summary>
        [Fact]
        public async Task ProcessDocument_ThrowsArgumentNullException_WhenDocumentIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _processor.ProcessDocument(null));
        }

        /// <summary>
        /// Verifica que procesar tipos soportados asigna el estado y resultado correctos
        /// </summary>
        [Theory]
        [InlineData("application/pdf", ProcessingStatus.Completed, "PDF procesado correctamente")]
        [InlineData("application/vnd.openxmlformats-officedocument.wordprocessingml.document", ProcessingStatus.Completed, "Documento Word procesado correctamente")]
        [InlineData("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ProcessingStatus.Completed, "Hoja de cálculo Excel procesada correctamente")]
        [InlineData("text/plain", ProcessingStatus.Completed, "Archivo de texto procesado correctamente")]
        public async Task ProcessDocument_SetsCorrectStatusAndResult_ForSupportedTypes(string contentType, ProcessingStatus expectedStatus, string expectedResult)
        {
            var doc = new Document { Id = Guid.NewGuid(), FileName = "file", ContentType = contentType };
            _mockRepo.Setup(r => r.UpdateAsync(doc)).Returns(Task.CompletedTask).Verifiable();
            await _processor.ProcessDocument(doc);
            Assert.Equal(expectedStatus, doc.Status);
            Assert.Equal(expectedResult, doc.ProcessingResult);
            _mockRepo.Verify(r => r.UpdateAsync(doc), Times.Once);
        }

        /// <summary>
        /// Verifica que procesar un tipo no soportado asigna estado Failed y resultado adecuado
        /// </summary>
        [Fact]
        public async Task ProcessDocument_SetsFailedStatusAndResult_ForUnsupportedType()
        {
            var doc = new Document { Id = Guid.NewGuid(), FileName = "file", ContentType = "application/unknown" };
            _mockRepo.Setup(r => r.UpdateAsync(doc)).Returns(Task.CompletedTask).Verifiable();
            await _processor.ProcessDocument(doc);
            Assert.Equal(ProcessingStatus.Failed, doc.Status);
            Assert.Equal("Tipo de documento no soportado", doc.ProcessingResult);
            _mockRepo.Verify(r => r.UpdateAsync(doc), Times.Once);
        }

        /// <summary>
        /// Verifica que CheckStatusAsync retorna el estado correcto del documento
        /// </summary>
        [Fact]
        public async Task CheckStatusAsync_ReturnsDocumentStatus()
        {
            var docId = Guid.NewGuid();
            var doc = new Document { Id = docId, FileName = "file", Status = ProcessingStatus.Processing };
            _mockRepo.Setup(r => r.GetByIdAsync(docId)).ReturnsAsync(doc);
            var status = await _processor.CheckStatusAsync(docId);
            Assert.Equal(ProcessingStatus.Processing, status);
        }

        /// <summary>
        /// Verifica que CheckStatusAsync retorna Failed cuando el documento no existe
        /// </summary>
        [Fact]
        public async Task CheckStatusAsync_ReturnsFailed_WhenDocumentNotFound()
        {
            var docId = Guid.NewGuid();
            _mockRepo.Setup(r => r.GetByIdAsync(docId)).ReturnsAsync((Document)null);
            var status = await _processor.CheckStatusAsync(docId);
            Assert.Equal(ProcessingStatus.Failed, status);
        }
    }
} 