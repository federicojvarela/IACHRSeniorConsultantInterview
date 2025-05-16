using Infrastructure;
using Microsoft.Extensions.Caching.Memory;

  namespace UnitTests.Storage
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
        public async Task GetAsyncShouldReturnNullWhenKeyDoesNotExist()
        {
            // Acción
            var resultObj = await _cacheService.GetAsync<string>("nonexistentKey");
            var result = resultObj as string;

            // Verificación
            Assert.Null(result);
        }

        /// <summary>
        /// Prueba que verifica que SetAsync almacena correctamente un valor en la caché
        /// </summary>
        [Fact]
        public async Task SetAsyncShouldStoreValueInCache()
        {
            // Preparación
            var key = "testKey";
            var value = "testValue";

            // Acción
            await _cacheService.SetAsync(key, value);

            // Verificación
            var cachedObj = await _cacheService.GetAsync<string>(key);
            var cachedValue = cachedObj as string;
            Assert.Equal(value, cachedValue);
        }

        /// <summary>
        /// Prueba que verifica que RemoveAsync elimina correctamente un valor de la caché
        /// </summary>
        [Fact]
        public async Task RemoveAsyncShouldRemoveValueFromCache()
        {
            // Preparación
            var key = "removableKey";
            var value = "valueToRemove";
            await _cacheService.SetAsync(key, value);

            // Acción
            await _cacheService.RemoveAsync(key);
            var resultObj = await _cacheService.GetAsync<string>(key);
            var result = resultObj as string;

            // Verificación
            Assert.Null(result);
        }

        /// <summary>
        /// Prueba que verifica que ClearAsync elimina todos los valores de la caché
        /// </summary>
        [Fact]
        public async Task ClearAsyncShouldRemoveAllValuesFromCache()
        {
            // Preparación
            await _cacheService.SetAsync("key1", "value1");
            await _cacheService.SetAsync("key2", "value2");

            // Acción
            await _cacheService.ClearAsync();

            // Verificación
            var result1 = await _cacheService.GetAsync<string>("key1");
            var result2 = await _cacheService.GetAsync<string>("key2");
            Assert.Null(result1);
            Assert.Null(result2);
        }

        /// <summary>
        /// Prueba que verifica que SetAsync expira correctamente un valor después de un tiempo
        /// </summary>
        [Fact]
        public async Task SetAsyncShouldExpireValueAfterTimespan()
        {
            await _cacheService.SetAsync("expiring", "value", TimeSpan.FromMilliseconds(100));
            await Task.Delay(200);
            var result = await _cacheService.GetAsync<string>("expiring");
            Assert.Null(result);
        }

        /// <summary>
        /// Prueba que verifica que SetAsync sobrescribe correctamente una clave existente
        /// </summary>
        [Fact]
        public async Task SetAsyncShouldOverwriteExistingKey()
        {
            await _cacheService.SetAsync("dup", "first");
            await _cacheService.SetAsync("dup", "second");
            var result = await _cacheService.GetAsync<string>("dup");
            Assert.Equal("second", result);
        }

        /// <summary>
        /// Prueba que verifica que RemoveAsync no lanza una excepción cuando la clave no existe
        /// </summary>
        [Fact]
        public async Task RemoveAsyncShouldNotThrowWhenKeyDoesNotExist()
        {
            var ex = await Record.ExceptionAsync(() => _cacheService.RemoveAsync("nope"));
            Assert.Null(ex);
        }
    }
}