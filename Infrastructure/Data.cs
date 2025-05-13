using Core.Entities;
using System.Text.Json;
using Core.Interfaces;

namespace Infrastructure.Data
{

    public class FileDocumentStorage : IDocumentStorage
    {
        private readonly string _basePath;
        private readonly Dictionary<Guid, Document> _documents;
        private readonly string _documentsFilePath;
        private readonly ICache _cache;
        private readonly LoggerServices _logger;

        public FileDocumentStorage(ICache cache, string basePath, LoggerServices logger)
        {
            _cache = cache;
            _basePath = basePath;
            _documentsFilePath = Path.Combine(_basePath, "documents.json");
            _logger = logger;

            Directory.CreateDirectory(_basePath);

            if (File.Exists(_documentsFilePath))
            {
                var json = File.ReadAllText(_documentsFilePath);
                _documents = JsonSerializer.Deserialize<Dictionary<Guid, Document>>(json)
                    ?? new Dictionary<Guid, Document>();
            }
            else
            {
                _documents = new Dictionary<Guid, Document>();
                SaveDocuments();
            }
        }

        public Document GetById(Guid id)
        {
            var cachedDocument = _cache.Get(id);
            if (cachedDocument != null) return cachedDocument;

            if (!_documents.ContainsKey(id))
            {
                _logger.LogError($"Documento con ID {id} no encontrado.");
                return null;    
            }

            var document = _documents[id];
            _cache.Set(id, document);
            return document;
        }

        public List<Document> GetAll()
        {
            return new List<Document>(_documents.Values);
        }

        public Document Save(Document document)
        {
            _documents[document.Id] = document;
            _cache.Set(document.Id, document);
            SaveDocuments();
            return document;
        }

        public void Update(Document document)
        {
            if (_documents.ContainsKey(document.Id))
            {
                _documents[document.Id] = document;
                _cache.Set(document.Id, document);
                SaveDocuments();
            }
        }

        public void Delete(Guid id)
        {
            if (_documents.ContainsKey(id))
            {
                _documents.Remove(id);
                _cache.Remove(id);
                SaveDocuments();
            }
        }

        private void SaveDocuments()
        {
            var json = JsonSerializer.Serialize(_documents);
            File.WriteAllText(_documentsFilePath, json);
        }

        public void InvalidateCache(Guid id)
        {
            _cache.Remove(id);
        }

        public void InvalidateAllCache()
        {
            _cache.Clear();
        }
    }

    public class InMemoryCache : ICache
    {
        private readonly Dictionary<Guid, Document> _cache = new();
        private readonly LoggerServices _loggerService;
        
        public InMemoryCache(LoggerServices loggerService)
        {
            _loggerService = loggerService;
        }

        public Document Get(Guid id)
        {
            if (!_cache.TryGetValue(id, out var doc))
            {
                _loggerService.LogError($"Documento con ID {id} no encontrado en la caché.");
                return null;
            }
            return doc;
        }
        public void Set(Guid id, Document document) => _cache[id] = document;
        public void Remove(Guid id) => _cache.Remove(id);
        public void Clear() => _cache.Clear();
    }
}
