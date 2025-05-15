namespace Core.Interfaces
{
    /// <summary>
    /// Define una interfaz para operaciones de caché
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Obtiene un valor de la caché de forma asíncrona
        /// </summary>
        /// <typeparam name="T">Tipo del valor a recuperar</typeparam>
        /// <param name="key">Clave única para identificar el valor en la caché</param>
        /// <returns>El valor almacenado en la caché, o null si no existe</returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Almacena un valor en la caché de forma asíncrona
        /// </summary>
        /// <typeparam name="T">Tipo del valor a almacenar</typeparam>
        /// <param name="key">Clave única para identificar el valor en la caché</param>
        /// <param name="value">Valor a almacenar en la caché</param>
        /// <param name="expiry">Tiempo de expiración opcional para el valor almacenado</param>
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

        /// <summary>
        /// Elimina un valor específico de la caché de forma asíncrona
        /// </summary>
        /// <param name="key">Clave del valor a eliminar</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// Limpia todos los valores almacenados en la caché de forma asíncrona
        /// </summary>
        Task ClearAsync();
    }
}
