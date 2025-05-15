using Core.Entities;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Document> GetByIdAsync(Guid id);
        Task<List<Document>> GetAllAsync();
        Task<Document> SaveAsync(Document document);
        Task UpdateAsync(Document document);
        Task DeleteAsync(Guid id);
    }
}