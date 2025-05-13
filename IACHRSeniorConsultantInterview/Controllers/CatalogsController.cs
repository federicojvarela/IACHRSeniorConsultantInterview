using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.DTOs;
namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogsController : ControllerBase
    {
        private readonly ICatalogRepository _catalogService;

        public CatalogsController(ICatalogRepository catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCatalogs() // Cambio a async
        {
            var catalogs = await _catalogService.GetAllCatalogsAsync(); // Uso metodo async
            var result = catalogs.Select(c => new CatalogDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ItemCount = c.Items.Count
            });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCatalog(string id) // Cambio a async
        {
            var catalog = await _catalogService.GetCatalogByIdAsync(id); // Uso metodo async
            if (catalog == null)
            {
                return NotFound();
            }

            // Mapear Catalog a CatalogDto
            var catalogDto = new CatalogDto
            {
                Id = catalog.Id,
                Name = catalog.Name,
                Description = catalog.Description,
                ItemCount = catalog.Items.Count
            };

            return Ok(catalogDto);
        }

        [HttpGet("{catalogId}/items/{itemId}")]
        public async Task<IActionResult> GetCatalogItem(string catalogId, string itemId) // Cambio a async
        {
            var item = await _catalogService.GetCatalogItemAsync(catalogId, itemId); // Uso metodo async
            if (item == null)
            {
                return NotFound();
            }

            // Mapear CatalogItem a CatalogItemDto
            var itemDto = new CatalogItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Value = item.Value
            };

            return Ok(itemDto);
        }
    }

}