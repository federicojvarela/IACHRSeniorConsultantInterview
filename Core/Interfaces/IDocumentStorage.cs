using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IDocumentStorage
    {
        Task<Document?> GetByIdAsync(Guid id);
        Task<List<Document>> GetAllAsync();
        Task<Document> SaveAsync(Document document);
        Task UpdateAsync(Document document);
        Task DeleteAsync(Guid id);
        Task InvalidateCacheAsync(Guid id);
        Task InvalidateAllCacheAsync();
    }
}