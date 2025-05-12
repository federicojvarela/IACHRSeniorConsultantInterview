using Core.Services;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetAllCatalogs()
        {
            var catalogs = _catalogService.GetAllCatalogs();
            return Ok(catalogs.Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                ItemCount = c.Items.Count
            }));
        }

        [HttpGet("{id}")]
        public IActionResult GetCatalog(string id)
        {
            var catalog = _catalogService.GetCatalog(id);
            if (catalog == null)
            {
                return NotFound();
            }

            return Ok(catalog);
        }

        [HttpGet("{catalogId}/items/{itemId}")]
        public IActionResult GetCatalogItem(string catalogId, string itemId)
        {
            var item = _catalogService.GetCatalogItem(catalogId, itemId);
            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }
    }
}
