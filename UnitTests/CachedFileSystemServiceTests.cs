using Infrastructure;
using Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;

namespace UnitTests.Services
{
    /// <summary>
    /// Pruebas unitarias para CachedFileSystemService, que verifica el comportamiento de caché sobre operaciones de archivos.
    /// </summary>
    public class CachedFileSystemServiceTests
    {
        /// <summary>
        /// Mock para el servicio de sistema de archivos subyacente.
        /// </summary>
        private readonly Mock<IFileSystemService> _inner;
        /// <summary>
        /// Servicio de caché en memoria utilizado en las pruebas.
        /// </summary>
        private readonly MemoryCacheService _cache;
        /// <summary>
        /// Instancia del servicio a testear.
        /// </summary>
        private readonly CachedFileSystemService _service;

        /// <summary>
        /// Inicializa los mocks y el servicio para los tests.
        /// </summary>
        public CachedFileSystemServiceTests()
        {
            _inner = new Mock<IFileSystemService>();
            _cache = new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()));
            _service = new CachedFileSystemService(_inner.Object, _cache);
        }

        /// <summary>
        /// Verifica que FileExistsAsync utiliza la caché y solo consulta el sistema de archivos una vez.
        /// </summary>
        [Fact]
        public async Task FileExistsShouldUseCache()
        {
            const string path = "exists.txt";
            _inner.Setup(f => f.FileExistsAsync(path)).ReturnsAsync(true);

            var first = await _service.FileExistsAsync(path);
            var second = await _service.FileExistsAsync(path);

            Assert.True(first);
            Assert.True(second);
            _inner.Verify(f => f.FileExistsAsync(path), Times.Once());
        }

        /// <summary>
        /// Verifica que ReadFileAsync cachea el contenido y solo lee una vez del sistema de archivos.
        /// </summary>
        [Fact]
        public async Task ReadFileAsyncShouldCacheContent()
        {
            const string path = "read.txt";
            _inner.Setup(f => f.ReadFileAsync(path)).ReturnsAsync("content");

            var first = await _service.ReadFileAsync(path);
            var second = await _service.ReadFileAsync(path);

            Assert.Equal("content", first);
            Assert.Equal("content", second);
            _inner.Verify(f => f.ReadFileAsync(path), Times.Once());
        }

        /// <summary>
        /// Verifica que WriteFileAsync refresca la caché correctamente.
        /// </summary>
        [Fact]
        public async Task WriteFileAsyncShouldRefreshCache()
        {
            const string path = "write.txt";
            _inner.Setup(f => f.WriteFileAsync(path, "new")).Returns(Task.CompletedTask);

            await _service.WriteFileAsync(path, "new");

            var exists = await _service.FileExistsAsync(path);
            _inner.Verify(f => f.FileExistsAsync(It.IsAny<string>()), Times.Never());

            var content = await _service.ReadFileAsync(path);
            _inner.Verify(f => f.ReadFileAsync(It.IsAny<string>()), Times.Never());

            Assert.True(exists);
            Assert.Equal("new", content);
        }
    }
}
