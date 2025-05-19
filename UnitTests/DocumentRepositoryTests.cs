using Infrastructure.Repositories;
using Infrastructure.Storage;
using Moq;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interfaces;

namespace UnitTests
{
    /// <summary>
    /// Pruebas unitarias para DocumentRepository, que verifica la delegación de operaciones al almacenamiento de documentos.
    /// </summary>
    public class DocumentRepositoryTests
    {
        /// <summary>
        /// Mock para el almacenamiento de documentos.
        /// </summary>
        private readonly Mock<IDocumentStorage> _mockStorage;
        /// <summary>
        /// Repositorio a testear.
        /// </summary>
        private readonly DocumentRepository _repo;

        /// <summary>
        /// Inicializa el mock y el repositorio para los tests.
        /// </summary>
        public DocumentRepositoryTests()
        {
            _mockStorage = new Mock<IDocumentStorage>();
            _repo = new DocumentRepository(_mockStorage.Object);
        }

        /// <summary>
        /// Verifica que GetAllAsync delega correctamente al almacenamiento.
        /// </summary>
        [Fact]
        public async Task GetAllAsync_DelegatesToStorage()
        {
            var expected = new List<Document> { new Document { Id = Guid.NewGuid(), FileName = "doc" } };
            _mockStorage.Setup(s => s.GetAllAsync()).ReturnsAsync(expected);
            var result = await _repo.GetAllAsync();
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Verifica que GetByIdAsync delega correctamente al almacenamiento.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_DelegatesToStorage()
        {
            var doc = new Document { Id = Guid.NewGuid(), FileName = "doc" };
            _mockStorage.Setup(s => s.GetByIdAsync(doc.Id)).ReturnsAsync(doc);
            var result = await _repo.GetByIdAsync(doc.Id);
            Assert.Equal(doc, result);
        }

        /// <summary>
        /// Verifica que SaveAsync delega correctamente al almacenamiento.
        /// </summary>
        [Fact]
        public async Task SaveAsync_DelegatesToStorage()
        {
            var doc = new Document { Id = Guid.NewGuid(), FileName = "doc" };
            _mockStorage.Setup(s => s.SaveAsync(doc)).ReturnsAsync(doc);
            var result = await _repo.SaveAsync(doc);
            Assert.Equal(doc, result);
        }

        /// <summary>
        /// Verifica que UpdateAsync delega correctamente al almacenamiento.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_DelegatesToStorage()
        {
            var doc = new Document { Id = Guid.NewGuid(), FileName = "doc" };
            _mockStorage.Setup(s => s.UpdateAsync(doc)).Returns(Task.CompletedTask).Verifiable();
            await _repo.UpdateAsync(doc);
            _mockStorage.Verify(s => s.UpdateAsync(doc), Times.Once);
        }

        /// <summary>
        /// Verifica que DeleteAsync delega correctamente al almacenamiento.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_DelegatesToStorage()
        {
            var id = Guid.NewGuid();
            _mockStorage.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask).Verifiable();
            await _repo.DeleteAsync(id);
            _mockStorage.Verify(s => s.DeleteAsync(id), Times.Once);
        }

        /// <summary>
        /// Verifica que DeleteAsync lanza ArgumentException si el id es Guid.Empty.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_ThrowsArgumentException_WhenIdIsEmpty()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _repo.DeleteAsync(Guid.Empty));
        }
    }
} 