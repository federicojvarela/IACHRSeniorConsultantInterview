using Infrastructure;
using Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Infrastructure.Tests
{
    /// <summary>
    /// Pruebas unitarias para el servicio de caché en memoria
    /// </summary>
    public class MemoryCacheServiceTests
    {
        private readonly MemoryCacheService _cacheService;

        /// <summary>
        /// Constructor que inicializa el servicio de caché para las pruebas
        /// </summary>
        public MemoryCacheServiceTests()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheService = new MemoryCacheService(memoryCache);
        }

        /// <summary>
        /// Prueba que verifica que GetAsync devuelve null cuando la clave no existe
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnNull_WhenKeyDoesNotExist()
        {
            // Acción
            var result = await _cacheService.GetAsync<string>("nonexistentKey");

            // Verificación
            Assert.Null(result);
        }

        /// <summary>
        /// Prueba que verifica que SetAsync almacena correctamente un valor en la caché
        /// </summary>
        [Fact]
        public async Task SetAsync_ShouldStoreValueInCache()
        {
            // Preparación
            var key = "testKey";
            var value = "testValue";

            // Acción
            await _cacheService.SetAsync(key, value);

            // Verificación
            var cachedValue = await _cacheService.GetAsync<string>(key);
            Assert.Equal(value, cachedValue);
        }

        /// <summary>
        /// Prueba que verifica que RemoveAsync elimina correctamente un valor de la caché
        /// </summary>
        [Fact]
        public async Task RemoveAsync_ShouldRemoveValueFromCache()
        {
            // Preparación
            var key = "removableKey";
            var value = "valueToRemove";
            await _cacheService.SetAsync(key, value);

            // Acción
            await _cacheService.RemoveAsync(key);
            var result = await _cacheService.GetAsync<string>(key);

            // Verificación
            Assert.Null(result);
        }

        /// <summary>
        /// Prueba que verifica que ClearAsync elimina todos los valores de la caché
        /// </summary>
        [Fact]
        public async Task ClearAsync_ShouldRemoveAllValuesFromCache()
        {
            // Preparación
            await _cacheService.SetAsync("key1", "value1");
            await _cacheService.SetAsync("key2", "value2");

            // Acción
            await _cacheService.ClearAsync();

            // Verificación
            Assert.Null(await _cacheService.GetAsync<string>("key1"));
            Assert.Null(await _cacheService.GetAsync<string>("key2"));
        }
    }
}