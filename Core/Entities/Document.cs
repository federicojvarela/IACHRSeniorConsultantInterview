using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.Entities
{
    /// <summary>
    /// Representa un documento en el sistema
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Identificador único del documento
        /// </summary>
        [Required(ErrorMessage = "Id es requerido.")]
        public required Guid Id { get; set; }

        /// <summary>
        /// Nombre del archivo del documento
        /// </summary>
        [Required(ErrorMessage = "Nombre del archivo requerido.")]
        public required string FileName { get; set; }

        /// <summary>
        /// Tipo de contenido MIME del documento
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Contenido binario del documento
        /// </summary>
        public byte[]? Content { get; set; }

        /// <summary>
        /// Fecha y hora en que se subió el documento
        /// </summary>
        public DateTime UploadDate { get; set; }

        /// <summary>
        /// Estado actual del procesamiento del documento
        /// </summary>
        public ProcessingStatus Status { get; set; }

        /// <summary>
        /// Resultado del procesamiento del documento, si está disponible
        /// </summary>
        public string? ProcessingResult { get; set; }
    }
}
