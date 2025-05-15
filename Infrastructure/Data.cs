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
       private readonly ILoggerService _logger;

       private readonly IFileSystemService _fileSystem;


        public FileDocumentStorage(ICache cache, ILoggerService logger, IFileSystemService fileSystem, string basePath)
        {
            _cache = cache;
            _basePath = basePath;
            _documentsFilePath = Path.Combine(_basePath, "documents.json");
            _logger = logger;
            _fileSystem = fileSystem;


            Directory.CreateDirectory(_basePath);

            _fileSystem.EnsureDirectoryExists(_basePath);
            if (_fileSystem.FileExists(_documentsFilePath))
             {
                var json = _fileSystem.ReadFileAsync(_documentsFilePath).Result;
                _documents = JsonSerializer.Deserialize<Dictionary<Guid, Document>>(json)
                    ?? new Dictionary<Guid, Document>();
            }
            else
            {
                _documents = new Dictionary<Guid, Document>();                
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


        public Task<List<Document>> GetAllAsync()
        {
            return Task.FromResult(_documents.Values.ToList());
        }


        public async Task<Document> SaveAsync(Document document)
        {
            _documents[document.Id] = document;
            var cacheKey = $"document_{document.Id}";
            await _cache.SetAsync(cacheKey, document, TimeSpan.FromMinutes(30));
            await SaveDocumentsAsync();
            return document;
        }

        public async Task UpdateAsync(Document document)
        {
            if (_documents.ContainsKey(document.Id))
            {
                _documents[document.Id] = document;
                var cacheKey = $"document_{document.Id}";
                await _cache.SetAsync(cacheKey, document, TimeSpan.FromMinutes(30));
                await SaveDocumentsAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            if (_documents.ContainsKey(id))
            {
                _documents.Remove(id);
                var cacheKey = $"document_{id}";
                await _cache.RemoveAsync(cacheKey);
                await SaveDocumentsAsync();
            }
        }

        private async Task SaveDocumentsAsync()
        {
            var json = JsonSerializer.Serialize(_documents);
            await _fileSystem.WriteFileAsync(_documentsFilePath, json);
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



}
