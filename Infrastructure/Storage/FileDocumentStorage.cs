using Core.Entities;
using Core.Interfaces;
using System.Text.Json;

namespace Infrastructure.Storage
{
    public class FileDocumentStorage : IDocumentStorage
    {
        private readonly ICache _cache;
        private readonly ILoggerService _logger;
        private readonly IFileSystemService _fileSystem;
        private readonly string _documentsFilePath;
        private readonly Dictionary<Guid, Document> _documents;

        public FileDocumentStorage(ICache cache, ILoggerService logger, IFileSystemService fileSystem, string basePath)
        {
            _cache = cache;
            _logger = logger;
            _fileSystem = fileSystem;
            _documentsFilePath = Path.Combine(basePath, "documents.json");

            _fileSystem.EnsureDirectoryExists(basePath);

            if (_fileSystem.FileExists(_documentsFilePath))
            {
                var json = _fileSystem.ReadFileAsync(_documentsFilePath).Result;
                _documents = JsonSerializer.Deserialize<Dictionary<Guid, Document>>(json) ?? new();
            }
            else
            {
                _documents = new();
            }
        }

        public async Task<Document?> GetByIdAsync(Guid id)
        {
            if (_cache != null)
            {
                var cached = await _cache.GetAsync<Document>($"document_{id}");
                if (cached != null) return cached;
            }

            if (_documents.TryGetValue(id, out var document))
            {
                await _cache.SetAsync($"document_{id}", document);
                return document;
            }

            _logger.LogWarning($"Documento con ID {id} no encontrado.");
            return null;
        }

        public Task<List<Document>> GetAllAsync()
        {
            return Task.FromResult(_documents.Values.ToList());
        }

        public async Task<Document> SaveAsync(Document document)
        {
            _documents[document.Id] = document;
            await _cache.SetAsync($"document_{document.Id}", document);
            await SaveDocumentsAsync();
            return document;
        }

        public async Task UpdateAsync(Document document)
        {
            if (_documents.ContainsKey(document.Id))
            {
                _documents[document.Id] = document;
                await _cache.SetAsync($"document_{document.Id}", document);
                await SaveDocumentsAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            if (_documents.Remove(id))
            {
                await _cache.RemoveAsync($"document_{id}");
                await SaveDocumentsAsync();
            }
        }

        public Task InvalidateCacheAsync(Guid id)
        {
            return _cache.RemoveAsync($"document_{id}");
        }

        public Task InvalidateAllCacheAsync()
        {
            return _cache.ClearAsync();
        }

        private async Task SaveDocumentsAsync()
        {
            var json = JsonSerializer.Serialize(_documents);
            await _fileSystem.WriteFileAsync(_documentsFilePath, json);
        }
    }
}
