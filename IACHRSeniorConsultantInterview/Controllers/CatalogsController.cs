using Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks; // Add this using directive

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogsController : ControllerBase
    {
        private readonly CatalogService _catalogService;

        public CatalogsController(CatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCatalogs() // Cambio a async
        {
            var catalogs = await _catalogService.GetAllCatalogsAsync(); // Uso metodo async
            return Ok(catalogs.Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                ItemCount = c.Items.Count
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCatalog(string id) // Cambio a async
        {
            var catalog = await _catalogService.GetCatalogAsync(id); // Uso metodo async
            if (catalog == null)
            {
                return NotFound();
            }

            return Ok(catalog);
        }

        [HttpGet("{catalogId}/items/{itemId}")]
        public async Task<IActionResult> GetCatalogItem(string catalogId, string itemId) // Cambio a async
        {
            var item = await _catalogService.GetCatalogItemAsync(catalogId, itemId); // Uso metodo async
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
    }
}