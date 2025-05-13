using Core.Entities;

namespace Core.Interfaces
{
    public interface ICache
    {
        Document Get(Guid id);
        void Set(Guid id, Document document);
        void Remove(Guid id);
        void Clear();
    }
}