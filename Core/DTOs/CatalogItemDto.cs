namespace Core.DTOs
{
    /// <summary>
    /// Representa un Objeto de Transferencia de Datos (DTO) para un elemento de catálogo.
    /// </summary>
    public class CatalogItemDto
    {
        /// <summary>
        /// Obtiene o establece el identificador único del elemento.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre descriptivo del elemento.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o establece el valor asociado al elemento del catálogo.
        /// Por ejemplo, para un tipo de documento podría ser el content-type.
        /// </summary>
        public string Value { get; set; }
    }
}