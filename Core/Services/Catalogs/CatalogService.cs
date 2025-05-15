using Core.Entities;
using Core.Interfaces;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Catalogs
{
   
    /// <summary>
    /// Servicio que maneja las operaciones relacionadas con los catálogos
    /// </summary>
    public class CatalogService
    {
        private readonly ICatalogRepository _catalogRepository;

        /// <summary>
        /// Constructor del servicio de catálogos
        /// </summary>
        /// <param name="catalogRepository">Repositorio de catálogos</param>
        public CatalogService(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        /// <summary>
        /// Obtiene un catálogo por su identificador de forma asíncrona
        /// </summary>
        /// <param name="id">Identificador único del catálogo</param>
        /// <returns>El catálogo encontrado</returns>
        public async Task<Catalog> GetCatalogAsync(string id)
        {
            return await _catalogRepository.GetCatalogByIdAsync(id);
        }

        /// <summary>
        /// Obtiene todos los catálogos de forma asíncrona
        /// </summary>
        /// <returns>Lista de todos los catálogos</returns>
        public async Task<List<Catalog>> GetAllCatalogsAsync()
        {
            return await _catalogRepository.GetAllCatalogsAsync();
        }

        /// <summary>
        /// Obtiene un elemento específico de un catálogo de forma asíncrona
        /// </summary>
        /// <param name="catalogId">Identificador del catálogo</param>
        /// <param name="itemId">Identificador del elemento</param>
        /// <returns>El elemento del catálogo encontrado</returns>
        public async Task<CatalogItem> GetCatalogItemAsync(string catalogId, string itemId)
        {
            return await _catalogRepository.GetCatalogItemAsync(catalogId, itemId);
        }
    }
}
