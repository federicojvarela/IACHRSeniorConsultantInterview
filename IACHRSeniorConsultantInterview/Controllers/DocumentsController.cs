using Core.Services.Documents;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controlador para operaciones relacionadas con documentos.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly DocumentService _documentService;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de documentos.
        /// </summary>
        /// <param name="documentService">Servicio de documentos a inyectar.</param>
        public DocumentsController(DocumentService documentService)
        {
            _documentService = documentService;
        }

        /// <summary>
        /// Obtiene un documento específico por su identificador.
        /// </summary>
        /// <param name="id">Identificador único del documento.</param>
        /// <returns>Documento solicitado o NotFound si no existe.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocument(Guid id)
        {
            var document = await _documentService.GetDocument(id);
            if (document == null)
            {
                return NotFound();
            }
            return Ok(new
            {
                document.Id,
                document.FileName,
                document.ContentType,
                document.UploadDate,
                document.Status,
                document.ProcessingResult
            });
        }

        /// <summary>
        /// Sube un nuevo documento al sistema.
        /// </summary>
        /// <param name="file">Archivo a subir.</param>
        /// <returns>Información del documento subido o BadRequest si hay error.</returns>
        [HttpPost]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No se ha subido ningún archivo");
            }

            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                var document = await _documentService.UploadDocumentAsync(
                    file.FileName,
                    file.ContentType,
                    memoryStream.ToArray()
                );

                return Ok(new
                {
                    document.Id,
                    document.FileName,
                    document.Status,
                    message = "Documento subido y procesado correctamente"
                });
            }
        }
    }
}
