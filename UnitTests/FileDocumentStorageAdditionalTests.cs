using System.Text.Json;
using Infrastructure.Storage;

namespace UnitTests.Storage
{
    public class FileDocumentStorageAdditionalTests
    {
        /// <summary>
        /// Mock para la caché
        /// </summary>
        private readonly Mock<ICache> _cacheMock;
        /// <summary>
        /// Mock para el logger
        /// </summary>
        private readonly Mock<ILoggerService> _loggerMock;
        /// <summary>
        /// Mock para el sistema de archivos
        /// </summary>
        private readonly Mock<IFileSystemService> _fileSystemMock;
        /// <summary>
        /// Ruta base temporal para pruebas
        /// </summary>
        private readonly string _testBasePath;
        /// <summary>
        /// Ruta al archivo de documentos temporal
        /// </summary>
        private readonly string _testFilePath;

        /// <summary>
        /// Inicializa los mocks y rutas para los tests
        /// </summary>
        public FileDocumentStorageAdditionalTests()
        {
            _cacheMock = new Mock<ICache>();
            _loggerMock = new Mock<ILoggerService>();
            _fileSystemMock = new Mock<IFileSystemService>();

            _testBasePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testBasePath);
            _testFilePath = Path.Combine(_testBasePath, "documents.json");

            _fileSystemMock.Setup(f => f.EnsureDirectoryExists(It.IsAny<string>()));
            _fileSystemMock.Setup(f => f.WriteFileAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        }

        /// <summary>
        /// Verifica que GetByIdAsync retorna null cuando el documento no existe
        /// </summary>
        [Fact]
        public async Task GetByIdAsyncShouldReturnNullWhenDocumentDoesNotExist()
        {
            var id = Guid.NewGuid();
            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);
            var result = await storage.GetByIdAsync(id);
            Assert.Null(result);
        }

        /// <summary>
        /// Verifica que InitAsync carga los documentos desde archivo
        /// </summary>
        [Fact]
        public async Task InitAsyncShouldLoadDocumentsFromFile()
        {
            var docId = Guid.NewGuid();
            var documents = new Dictionary<Guid, Document> { { docId, new Document { Id = docId, FileName = "LoadedDoc.txt" } } };
            var json = JsonSerializer.Serialize(documents);

            _fileSystemMock.Setup(f => f.FileExistsAsync(_testFilePath)).ReturnsAsync(true);
            _fileSystemMock.Setup(f => f.ReadFileAsync(_testFilePath)).ReturnsAsync(json);

            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);
            await storage.InitAsync();
            var result = await storage.GetByIdAsync(docId);

            Assert.NotNull(result);
            Assert.Equal("LoadedDoc.txt", result?.FileName);
        }

        /// <summary>
        /// Verifica que SaveAsync sobrescribe el documento si ya existe
        /// </summary>
        [Fact]
        public async Task SaveAsyncShouldOverwriteDocumentIfExists()
        {
            var docId = Guid.NewGuid();
            var doc1 = new Document { Id = docId, FileName = "Original.txt" };
            var doc2 = new Document { Id = docId, FileName = "Updated.txt" };

            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);
            await storage.SaveAsync(doc1);
            await storage.SaveAsync(doc2);
            var result = await storage.GetByIdAsync(docId);

            Assert.Equal("Updated.txt", result?.FileName);
        }

        /// <summary>
        /// Verifica que GetAllAsync retorna todos los documentos guardados
        /// </summary>
        [Fact]
        public async Task GetAllAsyncShouldReturnAllDocuments()
        {
            var doc1 = new Document { Id = Guid.NewGuid(), FileName = "Doc1.txt" };
            var doc2 = new Document { Id = Guid.NewGuid(), FileName = "Doc2.txt" };

            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);
            await storage.SaveAsync(doc1);
            await storage.SaveAsync(doc2);

            var allDocs = await storage.GetAllAsync();
            Assert.Equal(2, allDocs.Count);
        }

        /// <summary>
        /// Verifica que InvalidateCacheAsync elimina la clave correcta de la caché
        /// </summary>
        [Fact]
        public async Task InvalidateCacheAsyncShouldRemoveCorrectKey()
        {
            var docId = Guid.NewGuid();
            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);
            await storage.InvalidateCacheAsync(docId);

            _cacheMock.Verify(c => c.RemoveAsync($"document_{docId}"), Times.Once());
        }

        /// <summary>
        /// Verifica que UpdateAsync no lanza excepción si el documento no existe
        /// </summary>
        [Fact]
        public async Task UpdateAsyncShouldNotFailWhenDocumentDoesNotExist()
        {
            var doc = new Document { Id = Guid.NewGuid(), FileName = "Ghost.txt" };
            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);

            var exception = await Record.ExceptionAsync(() => storage.UpdateAsync(doc));
            Assert.Null(exception);
        }

        /// <summary>
        /// Verifica que DeleteAsync no hace nada si el id no existe
        /// </summary>
        [Fact]
        public async Task DeleteAsyncShouldDoNothingWhenIdNotFound()
        {
            var docId = Guid.NewGuid();
            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);

            await storage.DeleteAsync(docId);
            _cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.Never());
            _fileSystemMock.Verify(f => f.WriteFileAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }
    }  
}
