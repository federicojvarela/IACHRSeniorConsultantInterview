namespace Core.DTOs
{
    /// <summary>
    /// DTO que representa un catálogo con su información básica y lista de elementos.
    /// </summary>
    public class CatalogDto
    {
        /// <summary>
        /// Identificador único del catálogo.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nombre del catálogo.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descripción del catálogo.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Cantidad de elementos que contiene el catálogo.
        /// </summary>
        public int ItemCount { get; set; }

        /// <summary>
        /// Lista de elementos asociados al catálogo.
        /// </summary>
        public List<CatalogItemDto> Items { get; set; } = new();
    }
}    
    
