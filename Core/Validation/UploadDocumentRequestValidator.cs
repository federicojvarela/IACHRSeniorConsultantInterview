using FluentValidation;
using Core.DTOs;

namespace Core.Validation
{
    public class UploadDocumentRequestValidator : AbstractValidator<UploadDocumentRequest>
    {
        public UploadDocumentRequestValidator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("El nombre del archivo es obligatorio");

            RuleFor(x => x.Content)
                .NotNull().NotEmpty().WithMessage("Debe incluir el archivo");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("Debe especificar el tipo de contenido");
        }
    }
}
