using Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    /// <summary>
    /// Servicio que cachea la existencia y el contenido de archivos usando un <see cref="ICache"/>.
    /// Envuelve otra implementación de <see cref="IFileSystemService"/>.
    /// </summary>
    public class CachedFileSystemService : IFileSystemService
    {
        // Servicio de sistema de archivos subyacente (real)
        private readonly IFileSystemService _inner;
        // Implementación de caché utilizada para almacenar resultados
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

        // Genera la clave de caché para el contenido de un archivo
        private static string FileKey(string path) => $"file:{path}";
        // Genera la clave de caché para la existencia de un archivo
        private static string ExistsKey(string path) => $"exists:{path}";

        /// <inheritdoc />
        public async Task<bool> FileExistsAsync(string path)
        {
            // Busca en caché si ya se conoce la existencia del archivo
            var key = ExistsKey(path);
            var cached = await _cache.GetAsync<bool?>(key);
            if (cached.HasValue)
                return cached.Value;

            // Si no está en caché, consulta el sistema de archivos real y almacena el resultado
            var exists = await _inner.FileExistsAsync(path);
            await _cache.SetAsync(key, exists);
            return exists;
        }

        /// <inheritdoc />
        public async Task<string> ReadFileAsync(string path)
        {
            // Busca en caché el contenido del archivo
            var key = FileKey(path);
            var cached = await _cache.GetAsync<string>(key);
            if (cached is not null)
                return cached;

            // Si no está en caché, lee el archivo real y almacena el contenido en caché
            var content = await _inner.ReadFileAsync(path);
            await _cache.SetAsync(key, content);
            await _cache.SetAsync(ExistsKey(path), true); // También marca que el archivo existe
            return content;
        }

        /// <inheritdoc />
        public async Task WriteFileAsync(string path, string content)
        {
            // Escribe el archivo en el sistema real y actualiza la caché
            await _inner.WriteFileAsync(path, content);
            await _cache.SetAsync(FileKey(path), content);
            await _cache.SetAsync(ExistsKey(path), true);
        }

        /// <inheritdoc />
        public void EnsureDirectoryExists(string path)
        {
            // Asegura que el directorio exista usando el servicio real
            _inner.EnsureDirectoryExists(path);
        }
    }
}
