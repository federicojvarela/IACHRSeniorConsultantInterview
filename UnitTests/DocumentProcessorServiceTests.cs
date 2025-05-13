using Microsoft.Extensions.Logging;
using Core.Enums;

namespace UnitTests
{
    public class DocumentProcessorServiceTests
    {
        private readonly Mock<IDocumentRepository> _mockDocumentRepository;
        private readonly Mock<IDocumentProcessor> _mockDocumentProcessor;
        private readonly DocumentProcessorService _service;

        public DocumentProcessorServiceTests()
        {
            _mockDocumentRepository = new Mock<IDocumentRepository>();
            _mockDocumentProcessor = new Mock<IDocumentProcessor>();

           
            var loggerFactory = new LoggerFactory(); 
            var logger = loggerFactory.CreateLogger<LoggerServices>(); 
            var loggerService = new LoggerServices(logger); 

            _service = new DocumentProcessorService(
                _mockDocumentRepository.Object,
                _mockDocumentProcessor.Object,
                loggerService 
            );
        }

        [Fact]
        public void UploadDocument_ShouldSaveAndProcessDocument()
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

            // Setup mock para Save - guarda el documento inicial
            _mockDocumentRepository.Setup(r => r.Save(It.IsAny<Document>()))
                .Returns(savedDocument);

            // Setup mock para GetById - necesario para ProcessDocumentSync
            _mockDocumentRepository.Setup(r => r.GetById(documentId))
                .Returns(savedDocument);

            // Setup mock para Update - necesario para ProcessDocumentSync
            _mockDocumentRepository.Setup(r => r.Update(It.IsAny<Document>()))
                .Callback<Document>(d => savedDocument = d);

            // Act
            var result = _service.UploadDocument(fileName, contentType, content);

            // Assert
            Assert.Equal(savedDocument.Id, result.Id);
            Assert.Equal(fileName, result.FileName);
            Assert.Equal(contentType, result.ContentType);

            _mockDocumentRepository.Verify(r => r.Save(It.IsAny<Document>()), Times.Once);
            _mockDocumentProcessor.Verify(p => p.ProcessDocument(It.IsAny<Document>()), Times.Once);
        }

        [Fact]
        public void GetDocument_ShouldReturnDocumentFromRepository()
        {
            // Arrange
            var docId = Guid.NewGuid();
            var document = new Document { Id = docId, FileName = "test.pdf" };
            _mockDocumentRepository.Setup(r => r.GetById(docId)).Returns(document);

            // Act
            var result = _service.GetDocument(docId);

            // Assert
            Assert.Equal(document, result);
            _mockDocumentRepository.Verify(r => r.GetById(docId), Times.Once);
        }
    }
}
