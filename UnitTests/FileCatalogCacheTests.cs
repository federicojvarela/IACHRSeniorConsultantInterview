using Core.DTOs;
using Core.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using Xunit;

namespace UnitTests.Storage
{
    public class FileCatalogCacheTests
    {
        private readonly string _testDir;
        private readonly string _testFilePath;
        private readonly IMemoryCache _cache;

        public FileCatalogCacheTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDir);

            _testFilePath = Path.Combine(_testDir, "catalogs.json");
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        [Fact]
        public async Task GetCatalogShouldInvalidateCacheWhenFileChanges()
        {
            // Arrange: contenido inicial
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

            // Act 1: leer cache inicial
            var firstResult = catalogCache.GetCatalog();
            Assert.Single(firstResult);
            Assert.Equal("a", firstResult[0].Id);

            // Act 2: modificar archivo
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

            // Esperar a que FileSystemWatcher detecte el cambio
            await Task.Delay(500); // <- tiempo suficiente para que se invalide

            // Act 3: volver a leer el catálogo (debería venir nuevo)
            var secondResult = catalogCache.GetCatalog();
            Assert.Single(secondResult);
            Assert.Equal("b", secondResult[0].Id);
        }

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

        [Fact]
        public async Task RefreshAsyncShouldRepopulateCache()
        {
            // Arrange: create initial file
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

            // Act: call RefreshAsync and then GetCatalog
            await catalogCache.RefreshAsync();
            var result = catalogCache.GetCatalog();

            // Assert: the cache should be repopulated with the original data
            Assert.Single(result);
            Assert.Equal("a", result[0].Id);
        }
    }
}
