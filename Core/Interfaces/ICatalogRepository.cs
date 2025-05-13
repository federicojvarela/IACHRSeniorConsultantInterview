using Core.Entities;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface ICatalogRepository
    {
        Task<Catalog> GetCatalogByIdAsync(string id);
        Task<List<Catalog>> GetAllCatalogsAsync();
        Task<CatalogItem> GetCatalogItemAsync(string catalogId, string itemId);
    }
}