using Core.Entities;
using Core.Enums;

namespace Core.Interfaces
{
    public interface IDocumentProcessor
    {
        Task ProcessDocument(Document document);
        Task<ProcessingStatus> CheckStatusAsync(Guid documentId);
    }
}