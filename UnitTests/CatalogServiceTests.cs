using Core.Entities;
using Core.Interfaces;
using Core.Services;
using Moq;
using System.Collections.Generic;
using Xunit;

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
        public void GetCatalog_ShouldReturnCatalogFromRepository()
        {
            // Arrange
            var catalogId = "test-catalog";
            var catalog = new Catalog { Id = catalogId, Name = "Test Catalog" };
            _mockCatalogRepository.Setup(r => r.GetCatalogById(catalogId)).Returns(catalog);

            // Act
            var result = _service.GetCatalog(catalogId);

            // Assert
            Assert.Equal(catalog, result);
            _mockCatalogRepository.Verify(r => r.GetCatalogById(catalogId), Times.Once);
        }

        [Fact]
        public void GetAllCatalogs_ShouldReturnAllCatalogsFromRepository()
        {
            // Arrange
            var catalogs = new List<Catalog>
            {
                new Catalog { Id = "catalog1", Name = "Catalog 1" },
                new Catalog { Id = "catalog2", Name = "Catalog 2" }
            };
            _mockCatalogRepository.Setup(r => r.GetAllCatalogs()).Returns(catalogs);

            // Act
            var result = _service.GetAllCatalogs();

            // Assert
            Assert.Equal(catalogs, result);
            _mockCatalogRepository.Verify(r => r.GetAllCatalogs(), Times.Once);
        }

        [Fact]
        public void GetCatalogItem_ShouldReturnCatalogItemFromRepository()
        {
            // Arrange
            var catalogId = "test-catalog";
            var itemId = "test-item";
            var item = new CatalogItem { Id = itemId, Name = "Test Item" };
            _mockCatalogRepository.Setup(r => r.GetCatalogItem(catalogId, itemId)).Returns(item);

            // Act
            var result = _service.GetCatalogItem(catalogId, itemId);

            // Assert
            Assert.Equal(item, result);
            _mockCatalogRepository.Verify(r => r.GetCatalogItem(catalogId, itemId), Times.Once);
        }
    }
}