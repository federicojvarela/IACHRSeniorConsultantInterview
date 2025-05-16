using Core.Entities;
using Core.Interfaces;
using System.Text.Json;

namespace Infrastructure.Storage
{
    /// <summary>
    /// Implementación de almacenamiento de documentos usando el sistema de archivos y caché.
    /// Permite operaciones CRUD y mantiene los datos sincronizados en disco y memoria.
    /// </summary>
    public class FileDocumentStorage : IDocumentStorage
    {
        private readonly ICache _cache;
        private readonly ILoggerService _logger;
        private readonly IFileSystemService _fileSystem;
        private readonly string _documentsFilePath;
        private Dictionary<Guid, Document> _documents;

        /// <summary>
        /// Inicializa una nueva instancia de FileDocumentStorage.
        /// </summary>
        /// <param name="cache">Servicio de caché.</param>
        /// <param name="logger">Servicio de logging.</param>
        /// <param name="fileSystem">Servicio del sistema de archivos.</param>
        /// <param name="basePath">Ruta base donde se almacenarán los documentos.</param>
        public FileDocumentStorage(ICache cache, ILoggerService logger, IFileSystemService fileSystem, string basePath)
        {
            _cache = cache;
            _logger = logger;
            _fileSystem = fileSystem;
            _documentsFilePath = Path.Combine(basePath, "documents.json");
            _documents = new Dictionary<Guid, Document>();

            _fileSystem.EnsureDirectoryExists(basePath);

            InitAsync().Wait();
        }

        /// <summary>
        /// Inicializa el almacenamiento cargando los documentos desde el archivo en disco.
        /// </summary>
        public async Task InitAsync()
        {
            if (_fileSystem.FileExists(_documentsFilePath))
            {
                var json = await _fileSystem.ReadFileAsync(_documentsFilePath);
                _documents = JsonSerializer.Deserialize<Dictionary<Guid, Document>>(json) ?? new();
            }
            else
            {
                _documents = new();
            }
        }

        /// <summary>
        /// Obtiene un documento por su identificador único, usando caché si está disponible.
        /// </summary>
        /// <param name="id">Identificador único del documento.</param>
        /// <returns>El documento si existe, null en caso contrario.</returns>
        public async Task<Document?> GetByIdAsync(Guid id)
        {
            if (_cache != null)
            {
                var cachedObj = await _cache.GetAsync<Document>($"document_{id}");
                if (cachedObj is Document cached) return cached;
            }

            if (_documents.TryGetValue(id, out var document))
            {
                await _cache.SetAsync($"document_{id}", document);
                return document;
            }

            _logger.LogWarning($"Documento con ID {id} no encontrado.");
            return null;
        }

        /// <summary>
        /// Obtiene todos los documentos almacenados en memoria.
        /// </summary>
        /// <returns>Lista de todos los documentos.</returns>
        public Task<List<Document>> GetAllAsync()
        {
            return Task.FromResult(new List<Document>(_documents.Values));
        }

        /// <summary>
        /// Guarda un nuevo documento y lo persiste en disco y caché.
        /// </summary>
        /// <param name="document">Documento a guardar.</param>
        /// <returns>El documento guardado.</returns>
        public async Task<Document> SaveAsync(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            _documents[document.Id] = document;
            await _cache.SetAsync($"document_{document.Id}", document);
            await SaveDocumentsAsync();
            return document;
        }

        /// <summary>
        /// Actualiza un documento existente y sincroniza los cambios en disco y caché.
        /// </summary>
        /// <param name="document">Documento con los datos actualizados.</param>
        public async Task UpdateAsync(Document document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));
            if (_documents.ContainsKey(document.Id))
            {
                _documents[document.Id] = document;
                await _cache.SetAsync($"document_{document.Id}", document);
                await SaveDocumentsAsync();
            }
        }

        /// <summary>
        /// Elimina un documento por su identificador y actualiza disco y caché.
        /// </summary>
        /// <param name="id">Identificador del documento a eliminar.</param>
        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(id));
            if (_documents.Remove(id))
            {
                await _cache.RemoveAsync($"document_{id}");
                await SaveDocumentsAsync();
            }
        }

        /// <summary>
        /// Invalida la caché para un documento específico.
        /// </summary>
        /// <param name="id">Identificador del documento.</param>
        public Task InvalidateCacheAsync(Guid id)
        {
            return _cache.RemoveAsync($"document_{id}");
        }

        /// <summary>
        /// Invalida toda la caché de documentos.
        /// </summary>
        public Task InvalidateAllCacheAsync()
        {
            return _cache.ClearAsync();
        }

        /// <summary>
        /// Persiste todos los documentos en el archivo JSON en disco.
        /// </summary>
        private async Task SaveDocumentsAsync()
        {
            var json = JsonSerializer.Serialize(_documents);
            await _fileSystem.WriteFileAsync(_documentsFilePath, json);
        }
    }
}
