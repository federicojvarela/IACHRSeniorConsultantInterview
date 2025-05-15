using Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    /// <summary>
    /// Define una interfaz para operaciones de almacenamiento de documentos con caché
    /// </summary>
    public interface IDocumentStorage
    {
        /// <summary>
        /// Obtiene un documento por su identificador de forma asíncrona
        /// </summary>
        /// <param name="id">Identificador único del documento</param>
        /// <returns>El documento encontrado o null si no existe</returns>
        Task<Document?> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtiene todos los documentos almacenados de forma asíncrona
        /// </summary>
        /// <returns>Lista de todos los documentos</returns>
        Task<List<Document>> GetAllAsync();

        /// <summary>
        /// Guarda un nuevo documento de forma asíncrona
        /// </summary>
        /// <param name="document">El documento a guardar</param>
        /// <returns>El documento guardado con su identificador asignado</returns>
        Task<Document> SaveAsync(Document document);

        /// <summary>
        /// Actualiza un documento existente de forma asíncrona
        /// </summary>
        /// <param name="document">El documento con los datos actualizados</param>
        Task UpdateAsync(Document document);

        /// <summary>
        /// Elimina un documento por su identificador de forma asíncrona
        /// </summary>
        /// <param name="id">Identificador único del documento a eliminar</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Invalida la caché para un documento específico de forma asíncrona
        /// </summary>
        /// <param name="id">Identificador único del documento</param>
        Task InvalidateCacheAsync(Guid id);

        /// <summary>
        /// Invalida toda la caché de documentos de forma asíncrona
        /// </summary>
        Task InvalidateAllCacheAsync();
    }
}