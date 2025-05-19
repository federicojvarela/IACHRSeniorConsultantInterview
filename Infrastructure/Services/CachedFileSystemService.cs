using Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    /// <summary>
    /// Servicio que almacena en caché la existencia y el contenido de archivos usando un <see cref="ICache"/>.
    /// Envuelve otra implementación de <see cref="IFileSystemService"/>.
    /// </summary>
    public class CachedFileSystemService : IFileSystemService
    {
        private readonly IFileSystemService _inner;
        private readonly ICache _cache;

        /// <summary>
        /// Crea una nueva instancia de <see cref="CachedFileSystemService"/>.
        /// </summary>
        /// <param name="inner">Servicio de sistema de archivos subyacente.</param>
        /// <param name="cache">Implementación de caché.</param>
        public CachedFileSystemService(IFileSystemService inner, ICache cache)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        private static string FileKey(string path) => $"file:{path}";
        private static string ExistsKey(string path) => $"exists:{path}";

        /// <inheritdoc />
        /// <summary>
        /// Verifica si el archivo existe, utilizando la caché si es posible.
        /// </summary>
        public async Task<bool> FileExistsAsync(string path)
        {
            var key = ExistsKey(path);
            var cached = await _cache.GetAsync<bool?>(key);
            if (cached.HasValue)
                return cached.Value;

            var exists = await _inner.FileExistsAsync(path);
            await _cache.SetAsync(key, exists);
            return exists;
        }

        /// <inheritdoc />
        /// <summary>
        /// Lee el contenido de un archivo, utilizando la caché si es posible.
        /// </summary>
        public async Task<string> ReadFileAsync(string path)
        {
            var key = FileKey(path);
            var cached = await _cache.GetAsync<string>(key);
            if (cached is not null)
                return cached;

            var content = await _inner.ReadFileAsync(path);
            await _cache.SetAsync(key, content);
            await _cache.SetAsync(ExistsKey(path), true);
            return content;
        }

        /// <inheritdoc />
        /// <summary>
        /// Escribe el contenido en un archivo y actualiza la caché.
        /// </summary>
        public async Task WriteFileAsync(string path, string content)
        {
            await _inner.WriteFileAsync(path, content);
            await _cache.SetAsync(FileKey(path), content);
            await _cache.SetAsync(ExistsKey(path), true);
        }

        /// <inheritdoc />
        /// <summary>
        /// Asegura que el directorio exista, delegando en el servicio subyacente.
        /// </summary>
        public void EnsureDirectoryExists(string path)
        {
            _inner.EnsureDirectoryExists(path);
        }
    }
}
