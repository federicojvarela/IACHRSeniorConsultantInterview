using Core.Entities;
using Core.Interfaces;
using Infrastructure.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio que maneja las operaciones de documentos utilizando almacenamiento en archivos
    /// </summary>
    public class DocumentRepository : IDocumentRepository
    {
        private readonly FileDocumentStorage _storage;

        /// <summary>
        /// Constructor del repositorio de documentos
        /// </summary>
        /// <param name="storage">Servicio de almacenamiento de documentos en archivos</param>
        public DocumentRepository(FileDocumentStorage storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Obtiene todos los documentos de forma asíncrona
        /// </summary>
        /// <returns>Lista de todos los documentos almacenados</returns>
        public async Task<List<Document>> GetAllAsync()
        {
            return await _storage.GetAllAsync();
        }

        /// <summary>
        /// Obtiene un documento específico por su identificador de forma asíncrona
        /// </summary>
        /// <param name="id">Identificador único del documento</param>
        /// <returns>El documento encontrado o null si no existe</returns>
        public async Task<Document?> GetByIdAsync(Guid id)
        {
            return await _storage.GetByIdAsync(id);
        }

        /// <summary>
        /// Guarda un nuevo documento de forma asíncrona
        /// </summary>
        /// <param name="document">Documento a guardar</param>
        /// <returns>El documento guardado con su identificador actualizado</returns>
        public async Task<Document> SaveAsync(Document document)
        {
            return await _storage.SaveAsync(document);
        }

        /// <summary>
        /// Actualiza un documento existente de forma asíncrona
        /// </summary>
        /// <param name="document">Documento con los datos actualizados</param>
        public async Task UpdateAsync(Document document)
        {
            await _storage.UpdateAsync(document);
        }

        /// <summary>
        /// Elimina un documento por su identificador de forma asíncrona
        /// </summary>
        /// <param name="id">Identificador único del documento a eliminar</param>
        public async Task DeleteAsync(Guid id)
        {
            await _storage.DeleteAsync(id);
        }
    }
}
