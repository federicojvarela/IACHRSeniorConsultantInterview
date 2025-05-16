using System.ComponentModel.DataAnnotations;

namespace Core.DTOs
{
    /// <summary>
    /// DTO para la solicitud de carga de un documento.
    /// </summary>
    public class UploadDocumentRequest
    {
        /// <summary>
        /// Nombre del archivo a cargar.
        /// </summary>
        [Required(ErrorMessage = "El nombre del archivo es obligatorio.")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de contenido MIME del archivo.
        /// </summary>
        [Required(ErrorMessage = "El tipo de contenido es obligatorio.")]
        public string ContentType { get; set; } = "application/octet-stream";

        /// <summary>
        /// Contenido binario del archivo a cargar.
        /// </summary>
        [Required(ErrorMessage = "El contenido del archivo no puede estar vacío.")]
        public byte[] Content { get; set; } = Array.Empty<byte>();
    }
}
