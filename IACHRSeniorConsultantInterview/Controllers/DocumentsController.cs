using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly DocumentProcessorService _documentProcessorService;

        public DocumentsController(DocumentProcessorService documentProcessorService)
        {
            _documentProcessorService = documentProcessorService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocument(Guid id)
        {
            var document = await _documentProcessorService.GetDocument(id);
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
                var document = await _documentProcessorService.UploadDocumentAsync(
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
