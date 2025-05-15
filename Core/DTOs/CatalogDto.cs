namespace Core.DTOs
{
    /// <summary>
    /// Representa un Objeto de Transferencia de Datos (DTO) para un catálogo.
    /// </summary>
    public class CatalogDto
    {
        /// <summary>
        /// Obtiene o establece el identificador único para el catálogo.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del catálogo.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del catálogo.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Obtiene o establece la cantidad de elementos en el catálogo.
        /// </summary>
        public int ItemCount { get; set; }
    }
}    
    
