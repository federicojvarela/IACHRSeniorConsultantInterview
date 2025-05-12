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
        Catalog GetCatalogById(string id);
        List<Catalog> GetAllCatalogs();
        CatalogItem GetCatalogItem(string catalogId, string itemId);
    }
    #endregion

    public interface IDocumentProcessor
    {
        void ProcessDocument(Document document);
        ProcessingStatus CheckStatus(Guid documentId);
    }
}
