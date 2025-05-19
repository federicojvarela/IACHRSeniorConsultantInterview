using Infrastructure.Storage;

namespace UnitTests.Storage
{
    public class FileDocumentStorageTests
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
        public FileDocumentStorageTests()
        {
            _cacheMock = new Mock<ICache>();
            _loggerMock = new Mock<ILoggerService>();
            _fileSystemMock = new Mock<IFileSystemService>();

            _testBasePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testBasePath);
            _testFilePath = Path.Combine(_testBasePath, "documents.json");

            _fileSystemMock.Setup(f => f.EnsureDirectoryExists(It.IsAny<string>()));
            _fileSystemMock.Setup(f => f.FileExistsAsync(_testFilePath)).ReturnsAsync(false);
            _fileSystemMock.Setup(f => f.ReadFileAsync(_testFilePath)).ReturnsAsync("{}");
            _fileSystemMock.Setup(f => f.WriteFileAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        }

        /// <summary>
        /// Verifica que guardar y recuperar un documento usa la caché correctamente
        /// </summary>
        [Fact]
        public async Task SaveAndRetrieveDocumentShouldUseCache()
        {
            var doc = new Document { Id = Guid.NewGuid(), FileName = "TestDoc" };
            _cacheMock.Setup(c => c.GetAsync<Document>($"document_{doc.Id}")).ReturnsAsync((Document)null);
            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);

            await storage.SaveAsync(doc);
            _cacheMock.Setup(c => c.GetAsync<Document>($"document_{doc.Id}")).ReturnsAsync(doc);
            var result = await storage.GetByIdAsync(doc.Id);

            Assert.NotNull(result);
            Assert.Equal("TestDoc", result?.FileName);

            _cacheMock.Verify(c => c.SetAsync($"document_{doc.Id}", doc, null), Times.AtLeastOnce());
            _cacheMock.Verify(c => c.GetAsync<Document>($"document_{doc.Id}"), Times.AtLeastOnce());
        }

        /// <summary>
        /// Verifica que eliminar un documento invalida la caché correctamente
        /// </summary>
        [Fact]
        public async Task DeleteDocumentShouldInvalidateCache()
        {
            var docId = Guid.NewGuid();
            var doc = new Document { Id = docId, FileName = "ToDelete" };
            _cacheMock.Setup(c => c.GetAsync<Document>($"document_{docId}")).ReturnsAsync((Document)null);
            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);
            await storage.SaveAsync(doc);
            await storage.DeleteAsync(docId);

            _cacheMock.Verify(c => c.RemoveAsync($"document_{docId}"), Times.Once());
        }

        /// <summary>
        /// Verifica que actualizar un documento refresca la caché correctamente
        /// </summary>
        [Fact]
        public async Task UpdateDocumentShouldRefreshCache()
        {
            var docId = Guid.NewGuid();
            var doc = new Document { Id = docId, FileName = "Initial" };
            var updatedDoc = new Document { Id = docId, FileName = "Updated" };
            _cacheMock.Setup(c => c.GetAsync<Document>($"document_{docId}")).ReturnsAsync((Document)null);
            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);
            await storage.SaveAsync(doc);
            await storage.UpdateAsync(updatedDoc);

            _cacheMock.Verify(c => c.SetAsync($"document_{docId}", updatedDoc, null), Times.AtLeastOnce());
        }

        /// <summary>
        /// Verifica que InvalidateAllCacheAsync llama a ClearAsync en la caché
        /// </summary>
        [Fact]
        public async Task InvalidateAllCacheShouldCallClear()
        {
            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);
            await storage.InvalidateAllCacheAsync();

            _cacheMock.Verify(c => c.ClearAsync(), Times.Once());
        }

        /// <summary>
        /// Verifica que SaveAsync lanza excepción si el documento es null
        /// </summary>
        [Fact]
        public async Task SaveAsyncShouldThrowWhenDocumentIsNull()
        {
            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);
            await Assert.ThrowsAsync<ArgumentNullException>(() => storage.SaveAsync(null));
        }

        /// <summary>
        /// Verifica que UpdateAsync lanza excepción si el documento es null
        /// </summary>
        [Fact]
        public async Task UpdateAsyncShouldThrowWhenDocumentIsNull()
        {
            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);
            await Assert.ThrowsAsync<ArgumentNullException>(() => storage.UpdateAsync(null));
        }

        /// <summary>
        /// Verifica que DeleteAsync lanza excepción si el id es Guid.Empty
        /// </summary>
        [Fact]
        public async Task DeleteAsyncShouldThrowWhenIdIsEmpty()
        {
            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);
            await Assert.ThrowsAsync<ArgumentException>(() => storage.DeleteAsync(Guid.Empty));
        }

        /// <summary>
        /// Verifica que GetByIdAsync retorna null si el id es Guid.Empty
        /// </summary>
        [Fact]
        public async Task GetByIdAsyncShouldReturnNullWhenIdIsEmpty()
        {
            var storage = new FileDocumentStorage(_cacheMock.Object, _loggerMock.Object, _fileSystemMock.Object, _testBasePath);
            var result = await storage.GetByIdAsync(Guid.Empty);
            Assert.Null(result);
        }
    }
}
