using Core.Entities;
using Core.Interfaces;
using Core.Enums;
using System.Threading.Tasks;
using System;

namespace Infrastructure.Repositories
{
    public class SimpleDocumentProcessor : IDocumentProcessor
    {
        private readonly IDocumentRepository _documentRepository;

        public SimpleDocumentProcessor(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        public async Task ProcessDocument(Document document)
        {
            // Check if the document is null
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document), "El documento no puede ser nulo.");
            }

            // Simulación simple de procesamiento
            await Task.Delay(3000);

            // Actualizar el documento con el resultado del procesamiento
            document.Status = ProcessingStatus.Completed;

            switch (document.ContentType?.ToLower())
            {
                case "application/pdf":
                    document.ProcessingResult = "PDF procesado correctamente";
                    break;
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    document.ProcessingResult = "Documento Word procesado correctamente";
                    break;
                case "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    document.ProcessingResult = "Hoja de cálculo Excel procesada correctamente";
                    break;
                case "text/plain":
                    document.ProcessingResult = "Archivo de texto procesado correctamente";
                    break;
                default:
                    document.Status = ProcessingStatus.Failed;
                    document.ProcessingResult = "Tipo de documento no soportado";
                    break;
            }

            await _documentRepository.UpdateAsync(document);
        }

        public async Task<ProcessingStatus> CheckStatusAsync(Guid documentId)
        {
            var document = await _documentRepository.GetByIdAsync(documentId);
            return document?.Status ?? ProcessingStatus.Failed;
        }
    }
}
