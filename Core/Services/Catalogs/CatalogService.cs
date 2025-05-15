using Core.Entities;
using Core.Interfaces;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services.Catalogs
{
   

    public class CatalogService
    {
        private readonly ICatalogRepository _catalogRepository;

        public CatalogService(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        public async Task<Catalog> GetCatalogAsync(string id)
        {
            return await _catalogRepository.GetCatalogByIdAsync(id);
        }

        public async Task<List<Catalog>> GetAllCatalogsAsync()
        {
            return await _catalogRepository.GetAllCatalogsAsync();
        }

        public async Task<CatalogItem> GetCatalogItemAsync(string catalogId, string itemId)
        {
            return await _catalogRepository.GetCatalogItemAsync(catalogId, itemId);
        }
    }
}
