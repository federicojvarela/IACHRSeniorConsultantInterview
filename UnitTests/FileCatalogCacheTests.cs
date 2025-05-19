using Core.DTOs;
using Core.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using Xunit;

namespace UnitTests.Storage
{
    /// <summary>
    /// Pruebas unitarias para FileCatalogCache, que verifica el comportamiento de la caché de catálogos.
    /// </summary>
    public class FileCatalogCacheTests
    {
        /// <summary>
        /// Directorio temporal para pruebas.
        /// </summary>
        private readonly string _testDir;
        /// <summary>
        /// Ruta al archivo de catálogos temporal.
        /// </summary>
        private readonly string _testFilePath;
        /// <summary>
        /// Instancia de caché en memoria.
        /// </summary>
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Inicializa el entorno de pruebas creando un directorio y caché temporal.
        /// </summary>
        public FileCatalogCacheTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDir);

            _testFilePath = Path.Combine(_testDir, "catalogs.json");
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        /// <summary>
        /// Verifica que la caché se invalida cuando el archivo cambia y se recarga correctamente.
        /// </summary>
        [Fact]
        public async Task GetCatalogShouldInvalidateCacheWhenFileChanges()
        {
            var original = new[]
            {
                new CatalogDto
                {
                    Id = "test",
                    Name = "Original Catalog",
                    Description = "Test",
                    Items = new List<CatalogItemDto>
                    {
                        new CatalogItemDto { Id = "a", Name = "Item A", Value = "1" }
                    }
                }
            };
            File.WriteAllText(_testFilePath, JsonSerializer.Serialize(original));

            var catalogCache = new FileCatalogCache(_testDir, _cache);

            var firstResult = catalogCache.GetCatalog();
            Assert.Single(firstResult);
            Assert.Equal("a", firstResult[0].Id);

            var updated = new[]
            {
                new CatalogDto
                {
                    Id = "test",
                    Name = "Updated Catalog",
                    Description = "Test",
                    Items = new List<CatalogItemDto>
                    {
                        new CatalogItemDto { Id = "b", Name = "Item B", Value = "2" }
                    }
                }
            };
            File.WriteAllText(_testFilePath, JsonSerializer.Serialize(updated));

            await Task.Delay(500);

            var secondResult = catalogCache.GetCatalog();
            Assert.Single(secondResult);
            Assert.Equal("b", secondResult[0].Id);
        }

        /// <summary>
        /// Verifica que GetCatalog retorna una lista vacía cuando el archivo no existe.
        /// </summary>
        [Fact]
        public void GetCatalogShouldReturnEmptyListWhenFileMissing()
        {
            var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(dir);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var catalogCache = new FileCatalogCache(dir, cache);
            var result = catalogCache.GetCatalog();
            Assert.Empty(result);
        }

        /// <summary>
        /// Verifica que GetCatalogAsync retorna una lista vacía cuando el archivo no existe.
        /// </summary>
        [Fact]
        public async Task GetCatalogAsyncShouldReturnEmptyListWhenFileMissing()
        {
            var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(dir);
            var cache = new MemoryCache(new MemoryCacheOptions());
            var catalogCache = new FileCatalogCache(dir, cache);
            var result = await catalogCache.GetCatalogAsync();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// Verifica que RefreshAsync repuebla la caché correctamente.
        /// </summary>
        [Fact]
        public async Task RefreshAsyncShouldRepopulateCache()
        {
            var testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDir);
            var testFilePath = Path.Combine(testDir, "catalogs.json");
            var cache = new MemoryCache(new MemoryCacheOptions());
            var catalogCache = new FileCatalogCache(testDir, cache);

            var original = new[]
            {
                new CatalogDto
                {
                    Id = "test",
                    Name = "Original Catalog",
                    Description = "Test",
                    Items = new List<CatalogItemDto>
                    {
                        new CatalogItemDto { Id = "a", Name = "Item A", Value = "1" }
                    }
                }
            };
            File.WriteAllText(testFilePath, JsonSerializer.Serialize(original));

            await catalogCache.RefreshAsync();
            var result = catalogCache.GetCatalog();

            Assert.Single(result);
            Assert.Equal("a", result[0].Id);
        }
    }
}
