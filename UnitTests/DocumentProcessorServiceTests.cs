using Microsoft.Extensions.Logging;
using Core.Enums;
using Core.Services.Documents;

namespace UnitTests
{
    public class DocumentProcessorServiceTests
    {
        private readonly Mock<IDocumentRepository> _mockDocumentRepository;
        private readonly Mock<IDocumentProcessor> _mockDocumentProcessor;
        private readonly DocumentService _service;
        private readonly Mock<ICache> _mockCache;

        public DocumentProcessorServiceTests()
        {
            _mockDocumentRepository = new Mock<IDocumentRepository>();
            _mockDocumentProcessor = new Mock<IDocumentProcessor>();
            _mockCache = new Mock<ICache>();

            var loggerFactory = new LoggerFactory(); 
            var logger = loggerFactory.CreateLogger<LoggerServices>(); 
            var loggerService = new LoggerServices(logger); 

            _service = new DocumentService(
                _mockDocumentRepository.Object,
                _mockDocumentProcessor.Object,
                loggerService
            );
        }

        [Fact]
        public async Task UploadDocumentShouldSaveAndProcessDocument()
        {
            // Arrange
            var fileName = "test.pdf";
            var contentType = "application/pdf";
            var content = new byte[] { 1, 2, 3 };
            var documentId = Guid.NewGuid();
            var savedDocument = new Document
            {
                Id = documentId,
                FileName = fileName,
                ContentType = contentType,
                Content = content,
                Status = ProcessingStatus.Pending
            };

            // Setup mock para SaveAsync - guarda el documento inicial
            _mockDocumentRepository.Setup(r => r.SaveAsync(It.IsAny<Document>()))
                .ReturnsAsync(savedDocument);

            // Setup mock para GetByIdAsync - necesario para ProcessDocumentSync
            _mockDocumentRepository.Setup(r => r.GetByIdAsync(documentId))
                .ReturnsAsync(savedDocument);

            // Setup mock para Update - necesario para ProcessDocumentSync
            _mockDocumentRepository.Setup(r => r.UpdateAsync(It.IsAny<Document>()))
                .Callback<Document>(d => savedDocument = d);

            // Act
            var result = await _service.UploadDocumentAsync(fileName, contentType, content);

            // Assert
            Assert.Equal(savedDocument.Id, result.Id);
            Assert.Equal(fileName, result.FileName);
            Assert.Equal(contentType, result.ContentType);

            _mockDocumentRepository.Verify(r => r.SaveAsync(It.IsAny<Document>()), Times.Once);
            _mockDocumentProcessor.Verify(p => p.ProcessDocument(It.IsAny<Document>()), Times.Once);
        }

        [Fact]
        public async Task GetDocumentShouldReturnDocumentFromRepository()
        {
            // Arrange
            var docId = Guid.NewGuid();
            var document = new Document { Id = docId, FileName = "test.pdf" };
            _mockDocumentRepository.Setup(r => r.GetByIdAsync(docId)).ReturnsAsync(document);

            // Act
            var result = await _service.GetDocument(docId);

            // Assert
            Assert.Equal(document, result);
            _mockDocumentRepository.Verify(r => r.GetByIdAsync(docId), Times.Once);
        }
    }
}
