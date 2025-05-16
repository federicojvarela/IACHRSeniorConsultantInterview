using Infrastructure.Repositories;
using System.Text.Json;

namespace UnitTests
{
    public class CatalogRepositoryTests : IDisposable
    {
        private readonly string _tempFile;
        private readonly List<Catalog> _sampleCatalogs;

        public CatalogRepositoryTests()
        {
            _tempFile = Path.GetTempFileName();
            _sampleCatalogs = new List<Catalog>
            {
                new Catalog
                {
                    Id = "cat1",
                    Name = "Catalog 1",
                    Description = "Desc 1",
                    Items = new List<CatalogItem>
                    {
                        new CatalogItem { Id = "item1", Name = "Item 1", Value = "v1" },
                        new CatalogItem { Id = "item2", Name = "Item 2", Value = "v2" }
                    }
                },
                new Catalog
                {
                    Id = "cat2",
                    Name = "Catalog 2",
                    Description = "Desc 2",
                    Items = new List<CatalogItem>
                    {
                        new CatalogItem { Id = "item3", Name = "Item 3", Value = "v3" }
                    }
                }
            };
            File.WriteAllText(_tempFile, JsonSerializer.Serialize(_sampleCatalogs));
        }

        [Fact]
        public async Task GetAllCatalogsAsync_ReturnsAllCatalogs()
        {
            var repo = new CatalogRepository(_tempFile);
            var result = await repo.GetAllCatalogsAsync();
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetCatalogByIdAsync_ReturnsCorrectCatalog()
        {
            var repo = new CatalogRepository(_tempFile);
            var result = await repo.GetCatalogByIdAsync("cat1");
            Assert.Equal("cat1", result.Id);
            Assert.Equal("Catalog 1", result.Name);
        }

        [Fact]
        public async Task GetCatalogByIdAsync_ReturnsEmptyCatalog_WhenNotFound()
        {
            var repo = new CatalogRepository(_tempFile);
            var result = await repo.GetCatalogByIdAsync("notfound");
            Assert.NotNull(result);
            Assert.Null(result.Id);
        }

        [Fact]
        public async Task GetCatalogItemAsync_ReturnsCorrectItem()
        {
            var repo = new CatalogRepository(_tempFile);
            var result = await repo.GetCatalogItemAsync("cat1", "item2");
            Assert.Equal("item2", result.Id);
            Assert.Equal("Item 2", result.Name);
        }

        [Fact]
        public async Task GetCatalogItemAsync_ReturnsEmptyItem_WhenNotFound()
        {
            var repo = new CatalogRepository(_tempFile);
            var result = await repo.GetCatalogItemAsync("cat1", "notfound");
            Assert.NotNull(result);
            Assert.Null(result.Id);
        }

        [Fact]
        public void LoadCatalogs_DoesNotThrow_OnInvalidFile()
        {
            var badFile = Path.GetTempFileName();
            File.WriteAllText(badFile, "not a json");
            var ex = Record.Exception(() => new CatalogRepository(badFile));
            Assert.Null(ex); // Should handle error gracefully
            File.Delete(badFile);
        }

        public void Dispose()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }
    }
} 