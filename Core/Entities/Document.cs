using System.ComponentModel.DataAnnotations;
using Core.Enums;

namespace Core.Entities
{
    public class Document
    {
        [Required(ErrorMessage = "Id es requerido.")]
        public required Guid Id { get; set; }

        [Required(ErrorMessage = "Nombre del archivo requerido.")]
        public required string FileName { get; set; }

        public string? ContentType { get; set; }

        public byte[]? Content { get; set; }

        public DateTime UploadDate { get; set; }
        public ProcessingStatus Status { get; set; }
        public string? ProcessingResult { get; set; }
    }
}
