using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    #region Repositories
    public interface IDocumentRepository
    {
        Document GetById(Guid id);
        List<Document> GetAll();
        Document Save(Document document);
        void Update(Document document);
        void Delete(Guid id);
    }

    public interface ICatalogRepository
    {
        Task<Catalog> GetCatalogByIdAsync(string id);
        Task<List<Catalog>> GetAllCatalogsAsync();
        Task<CatalogItem> GetCatalogItemAsync(string catalogId, string itemId);
    }
    #endregion

    public interface IDocumentProcessor
    {
        void ProcessDocument(Document document);
        ProcessingStatus CheckStatus(Guid documentId);
    }
}
