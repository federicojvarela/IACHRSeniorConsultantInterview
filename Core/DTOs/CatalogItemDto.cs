namespace Core.DTOs
{
    /// <summary>
    /// DTO que representa un elemento individual dentro de un catálogo.
    /// </summary>
    public class CatalogItemDto
    {
        /// <summary>
        /// Identificador único del elemento del catálogo.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nombre descriptivo del elemento.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Valor asociado al elemento del catálogo (por ejemplo, content-type).
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Nombre del archivo asociado al elemento (si aplica).
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Ruta del archivo asociado al elemento (si aplica).
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Tamaño del archivo asociado al elemento (en bytes).
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Fecha de última modificación del archivo asociado al elemento.
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}