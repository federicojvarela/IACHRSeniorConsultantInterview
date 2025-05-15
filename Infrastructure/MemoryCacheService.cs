using Microsoft.Extensions.Caching.Memory;
using Core.Interfaces;

namespace Infrastructure
{
    public class MemoryCacheService : ICache
    {
        private readonly IMemoryCache _cache;
        private readonly List<string> _keys;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
            _keys = new List<string>();
        }

        public Task<T?> GetAsync<T>(string key)
        {
            _cache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var options = expiry.HasValue ? new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiry } : null;
            _cache.Set(key, value, options);
            _keys.Add(key);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            _keys.Remove(key);
            return Task.CompletedTask;
        }

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