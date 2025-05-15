
using Core.Services.Catalogs;

namespace UnitTests
{
    public class CatalogServiceTests
    {
        private readonly Mock<ICatalogRepository> _mockCatalogRepository;
        private readonly CatalogService _service;

        public CatalogServiceTests()
        {
            _mockCatalogRepository = new Mock<ICatalogRepository>();
            _service = new CatalogService(_mockCatalogRepository.Object);
        }

        [Fact]
        public async Task GetCatalog_ShouldReturnCatalogFromRepository()
        {
            // Arrange
            var catalogId = "test-catalog";
            var catalog = new Catalog { Id = catalogId, Name = "Test Catalog" };
            _mockCatalogRepository.Setup(r => r.GetCatalogByIdAsync(catalogId)).ReturnsAsync(catalog);

            // Act
            var result = await _service.GetCatalogAsync(catalogId);

            // Assert
            Assert.Equal(catalog, result);
            _mockCatalogRepository.Verify(r => r.GetCatalogByIdAsync(catalogId), Times.Once);
        }

        [Fact]
        public async Task GetAllCatalogs_ShouldReturnAllCatalogsFromRepository()
        {
            // Arrange
            var catalogs = new List<Catalog>
            {
                new Catalog { Id = "catalog1", Name = "Catalog 1" },
                new Catalog { Id = "catalog2", Name = "Catalog 2" }
            };
            _mockCatalogRepository.Setup(r => r.GetAllCatalogsAsync()).ReturnsAsync(catalogs);

            // Act
            var result = await _service.GetAllCatalogsAsync();

            // Assert
            Assert.Equal(catalogs, result);
            _mockCatalogRepository.Verify(r => r.GetAllCatalogsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCatalogItem_ShouldReturnCatalogItemFromRepository()
        {
            // Arrange
            var catalogId = "test-catalog";
            var itemId = "test-item";
            var item = new CatalogItem { Id = itemId, Name = "Test Item" };
            _mockCatalogRepository.Setup(r => r.GetCatalogItemAsync(catalogId, itemId)).ReturnsAsync(item);

            // Act
            var result = await _service.GetCatalogItemAsync(catalogId, itemId);

            // Assert
            Assert.Equal(item, result);
            _mockCatalogRepository.Verify(r => r.GetCatalogItemAsync(catalogId, itemId), Times.Once);
        }
    }
}