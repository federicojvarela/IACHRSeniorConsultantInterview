using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.DTOs;
namespace WebApi.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con los catálogos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogsController : ControllerBase
    {
        // Repositorio de catálogos inyectado por dependencia
        private readonly ICatalogRepository _catalogService;

        /// <summary>
        /// Constructor del controlador de catálogos
        /// </summary>
        /// <param name="catalogService">Servicio de catálogos a inyectar</param>
        public CatalogsController(ICatalogRepository catalogService)
        {
            _catalogService = catalogService;
        }

        /// <summary>
        /// Obtiene todos los catálogos disponibles
        /// </summary>
        /// <returns>Lista de catálogos en formato DTO</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCatalogs()
        {
            // Obtiene todos los catálogos de forma asíncrona
            var catalogs = await _catalogService.GetAllCatalogsAsync();
            
            // Mapea los catálogos a DTOs para la respuesta
            var result = catalogs.Select(c => new CatalogDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                ItemCount = c.Items.Count
            });
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un catálogo específico por su identificador
        /// </summary>
        /// <param name="id">Identificador único del catálogo</param>
        /// <returns>Catálogo solicitado en formato DTO</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCatalog(string id)
        {
            // Busca el catálogo por su ID de forma asíncrona
            var catalog = await _catalogService.GetCatalogByIdAsync(id);
            if (catalog == null)
            {
                return NotFound(); // Retorna 404 si no se encuentra el catálogo
            }

            // Mapea el catálogo a DTO para la respuesta
            var catalogDto = new CatalogDto
            {
                Id = catalog.Id,
                Name = catalog.Name,
                Description = catalog.Description,
                ItemCount = catalog.Items.Count
            };

            return Ok(catalogDto);
        }

        /// <summary>
        /// Obtiene un elemento específico de un catálogo
        /// </summary>
        /// <param name="catalogId">Identificador del catálogo</param>
        /// <param name="itemId">Identificador del elemento</param>
        /// <returns>Elemento del catálogo en formato DTO</returns>
        [HttpGet("{catalogId}/items/{itemId}")]
        public async Task<IActionResult> GetCatalogItem(string catalogId, string itemId)
        {
            // Busca el elemento específico en el catálogo de forma asíncrona
            var item = await _catalogService.GetCatalogItemAsync(catalogId, itemId);
            if (item == null)
            {
                return NotFound(); // Retorna 404 si no se encuentra el elemento
            }

            // Mapea el elemento a DTO para la respuesta
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