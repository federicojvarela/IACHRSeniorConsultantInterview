using Core.Services.Documents;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly DocumentService _documentService;

        public DocumentsController(DocumentService documentService)
        {
            _documentService = documentService;
        }

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
