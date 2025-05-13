using Core.Entities;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IDocumentRepository
    {
        Document GetById(Guid id);
        List<Document> GetAll();
        Document Save(Document document);
        void Update(Document document);
        void Delete(Guid id);
    }
}