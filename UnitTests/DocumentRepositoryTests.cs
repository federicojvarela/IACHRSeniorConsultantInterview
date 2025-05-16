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
    public class DocumentRepositoryTests
    {
        private readonly Mock<IDocumentStorage> _mockStorage;
        private readonly DocumentRepository _repo;

        public DocumentRepositoryTests()
        {
            _mockStorage = new Mock<IDocumentStorage>();
            _repo = new DocumentRepository(_mockStorage.Object);
        }

        [Fact]
        public async Task GetAllAsync_DelegatesToStorage()
        {
            var docs = new List<Document> { new Document { Id = Guid.NewGuid(), FileName = "file" } };
            _mockStorage.Setup(s => s.GetAllAsync()).ReturnsAsync(docs);
            var result = await _repo.GetAllAsync();
            Assert.Equal(docs, result);
        }

        [Fact]
        public async Task GetByIdAsync_DelegatesToStorage()
        {
            var doc = new Document { Id = Guid.NewGuid(), FileName = "file" };
            _mockStorage.Setup(s => s.GetByIdAsync(doc.Id)).ReturnsAsync(doc);
            var result = await _repo.GetByIdAsync(doc.Id);
            Assert.Equal(doc, result);
        }

        [Fact]
        public async Task SaveAsync_DelegatesToStorage()
        {
            var doc = new Document { Id = Guid.NewGuid(), FileName = "file" };
            _mockStorage.Setup(s => s.SaveAsync(doc)).ReturnsAsync(doc);
            var result = await _repo.SaveAsync(doc);
            Assert.Equal(doc, result);
        }

        [Fact]
        public async Task UpdateAsync_DelegatesToStorage()
        {
            var doc = new Document { Id = Guid.NewGuid(), FileName = "file" };
            _mockStorage.Setup(s => s.UpdateAsync(doc)).Returns(Task.CompletedTask).Verifiable();
            await _repo.UpdateAsync(doc);
            _mockStorage.Verify(s => s.UpdateAsync(doc), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DelegatesToStorage()
        {
            var docId = Guid.NewGuid();
            _mockStorage.Setup(s => s.DeleteAsync(docId)).Returns(Task.CompletedTask).Verifiable();
            await _repo.DeleteAsync(docId);
            _mockStorage.Verify(s => s.DeleteAsync(docId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsArgumentException_WhenIdIsEmpty()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _repo.DeleteAsync(Guid.Empty));
        }
    }
} 