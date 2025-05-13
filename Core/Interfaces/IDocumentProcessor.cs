using Core.Entities;
using Core.Enums;

namespace Core.Interfaces
{
    public interface IDocumentProcessor
    {
        void ProcessDocument(Document document);
        ProcessingStatus CheckStatus(Guid documentId);
    }
}