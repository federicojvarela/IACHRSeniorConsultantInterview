using Core.DTOs;

namespace Core.Interfaces
{
    /// <summary>
    /// Interfaz para operaciones de caché específicas de catálogos.
    /// </summary>
    public interface ICatalogCache
    {
        /// <summary>
        /// Obtiene la lista de elementos del catálogo desde la caché de manera síncrona.
        /// </summary>
        /// <returns>Lista de elementos del catálogo.</returns>
        List<CatalogItemDto> GetCatalog();

        /// <summary>
        /// Obtiene la lista de elementos del catálogo desde la caché de manera asíncrona.
        /// </summary>
        /// <returns>Lista de elementos del catálogo.</returns>
        Task<List<CatalogItemDto>?> GetCatalogAsync();

        /// <summary>
        /// Fuerza la recarga de la caché del catálogo de manera asíncrona.
        /// </summary>
        Task RefreshAsync();
    }
}
