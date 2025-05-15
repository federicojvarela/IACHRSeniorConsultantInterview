using Core.Entities;
using System.Text.Json;
using Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<Document?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"document_{id}";
            var cachedDocument = await _cache.GetAsync<Document>(cacheKey);
            if (cachedDocument != null) return cachedDocument;

            if (!_documents.ContainsKey(id))
            {
                _logger.LogError($"Documento con ID {id} no encontrado.");
                return null;
            }

            var document = _documents[id];
            await _cache.SetAsync(cacheKey, document, TimeSpan.FromMinutes(30));
            return document;
        }


        public async Task<List<Document>> GetAllAsync()
        {
            return await Task.Run(() => new List<Document>(_documents.Values));
        }

        public async Task<Document> Save(Document document)
        {
            _documents[document.Id] = document;
            var cacheKey = $"document_{document.Id}";
            await _cache.SetAsync(cacheKey, document, TimeSpan.FromMinutes(30));
            SaveDocuments();
            return document;
        }

        public async Task Update(Document document)
        {
            if (_documents.ContainsKey(document.Id))
            {
                _documents[document.Id] = document;
                var cacheKey = $"document_{document.Id}";
                await _cache.SetAsync(cacheKey, document, TimeSpan.FromMinutes(30));
                SaveDocuments();
            }
        }

        public async Task Delete(Guid id)
        {
            if (_documents.ContainsKey(id))
            {
                _documents.Remove(id);
                var cacheKey = $"document_{id}";
                await _cache.RemoveAsync(cacheKey);
                SaveDocuments();
            }
        }

        public async Task<Document> SaveAsync(Document document)
        {
            _documents[document.Id] = document;
            var cacheKey = $"document_{document.Id}";
            await _cache.SetAsync(cacheKey, document, TimeSpan.FromMinutes(30));
            SaveDocuments();
            return document;
        }

        public async Task UpdateAsync(Document document)
        {
            if (_documents.ContainsKey(document.Id))
            {
                _documents[document.Id] = document;
                var cacheKey = $"document_{document.Id}";
                await _cache.SetAsync(cacheKey, document, TimeSpan.FromMinutes(30));
                SaveDocuments();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            if (_documents.ContainsKey(id))
            {
                _documents.Remove(id);
                var cacheKey = $"document_{id}";
                await _cache.RemoveAsync(cacheKey);
                SaveDocuments();
            }
        }

        private void SaveDocuments()
        {
            var json = JsonSerializer.Serialize(_documents);
            File.WriteAllText(_documentsFilePath, json);
        }

        public async Task InvalidateCacheAsync(Guid id)
        {
            await _cache.RemoveAsync($"document_{id}");
        }

        public async Task InvalidateAllCacheAsync()
        {
            await _cache.ClearAsync();
        }
    }

public class InMemoryCache : ICache
{
    private readonly Dictionary<string, object> _cache = new();
    private readonly LoggerServices _loggerService;

    public InMemoryCache(LoggerServices loggerService)
    {
        _loggerService = loggerService;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        if (_cache.TryGetValue(key, out var value) && value is T typed)
        {
            return Task.FromResult<T?>(typed);
        }

        _loggerService.LogError($"Elemento con clave {key} no encontrado en la caché.");
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        _cache[key] = value!;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        _cache.Clear();
        return Task.CompletedTask;
    }
}

}
