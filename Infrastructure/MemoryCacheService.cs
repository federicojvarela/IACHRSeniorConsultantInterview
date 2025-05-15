using Microsoft.Extensions.Caching.Memory;
using Core.Interfaces;

namespace Infrastructure
{
    /// <summary>
    /// Servicio de caché en memoria que implementa la interfaz ICache
    /// </summary>
    public class MemoryCacheService : ICache
    {
        private readonly IMemoryCache _cache;
        private readonly List<string> _keys;

        /// <summary>
        /// Constructor del servicio de caché en memoria
        /// </summary>
        /// <param name="memoryCache">Instancia de IMemoryCache a utilizar</param>
        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _keys = new List<string>();
        }

        /// <summary>
        /// Obtiene un valor de la caché de forma asíncrona
        /// </summary>
        /// <typeparam name="T">Tipo del valor a obtener</typeparam>
        /// <param name="key">Clave del valor en la caché</param>
        /// <returns>El valor almacenado o null si no existe</returns>
        public Task<T?> GetAsync<T>(string key)
        {
            _cache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        /// <summary>
        /// Almacena un valor en la caché de forma asíncrona
        /// </summary>
        /// <typeparam name="T">Tipo del valor a almacenar</typeparam>
        /// <param name="key">Clave para almacenar el valor</param>
        /// <param name="value">Valor a almacenar</param>
        /// <param name="expiry">Tiempo de expiración opcional</param>
        public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var options = expiry.HasValue ? new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry } : null;
            _cache.Set(key, value, options);
            _keys.Add(key);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Elimina un valor de la caché de forma asíncrona
        /// </summary>
        /// <param name="key">Clave del valor a eliminar</param>
        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            _keys.Remove(key);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Limpia todos los valores almacenados en la caché de forma asíncrona
        /// </summary>
        public Task ClearAsync()
        {
            foreach (var key in _keys)
            {
                _cache.Remove(key);
            }
            _keys.Clear();
            return Task.CompletedTask;
        }
    }
}