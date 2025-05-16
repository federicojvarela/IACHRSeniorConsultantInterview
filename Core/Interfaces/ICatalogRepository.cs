using Core.Entities;
using System.Collections.Generic;

namespace Core.Interfaces
{
    /// <summary>
    /// Interfaz para operaciones de acceso y consulta de catálogos en el repositorio.
    /// </summary>
    public interface ICatalogRepository
    {
        /// <summary>
        /// Obtiene un catálogo por su identificador de forma asíncrona.
        /// </summary>
        /// <param name="id">Identificador único del catálogo.</param>
        /// <returns>El catálogo encontrado.</returns>
        Task<Catalog> GetCatalogByIdAsync(string id);

        /// <summary>
        /// Obtiene todos los catálogos disponibles de forma asíncrona.
        /// </summary>
        /// <returns>Lista de todos los catálogos.</returns>
        Task<List<Catalog>> GetAllCatalogsAsync();

        /// <summary>
        /// Obtiene un elemento específico de un catálogo de forma asíncrona.
        /// </summary>
        /// <param name="catalogId">Identificador del catálogo.</param>
        /// <param name="itemId">Identificador del elemento.</param>
        /// <returns>El elemento del catálogo encontrado.</returns>
        Task<CatalogItem> GetCatalogItemAsync(string catalogId, string itemId);
    }
}