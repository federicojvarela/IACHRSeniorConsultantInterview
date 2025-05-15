using System.ComponentModel.DataAnnotations;

namespace Core.DTOs
{
    public class UploadDocumentRequest
    {
        [Required(ErrorMessage = "El nombre del archivo es obligatorio.")]
        public string FileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de contenido es obligatorio.")]
        public string ContentType { get; set; } = "application/octet-stream";

        [Required(ErrorMessage = "El contenido del archivo no puede estar vacío.")]
        public byte[] Content { get; set; } = Array.Empty<byte>();
    }
}
