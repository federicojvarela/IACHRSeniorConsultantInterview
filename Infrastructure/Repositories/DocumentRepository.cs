using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly FileDocumentStorage _storage;

        public DocumentRepository(FileDocumentStorage storage)
        {
            _storage = storage;
        }

        public async Task<List<Document>> GetAllAsync()
        {
            return await _storage.GetAllAsync();
        }

        public async Task<Document?> GetByIdAsync(Guid id)
        {
            return await _storage.GetByIdAsync(id);
        }

        public async Task<Document> SaveAsync(Document document)
        {
            return await _storage.SaveAsync(document);
        }

        public async Task UpdateAsync(Document document)
        {
            await _storage.UpdateAsync(document);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _storage.DeleteAsync(id);
        }
    }
}
