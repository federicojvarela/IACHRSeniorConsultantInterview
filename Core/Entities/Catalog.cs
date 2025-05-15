using System.Text.Json.Serialization;

namespace Core.Entities
{
    /// <summary>
    /// Representa un catálogo que contiene una colección de elementos categorizados
    /// </summary>
    public class Catalog
    {
        /// <summary>
        /// Identificador único del catálogo
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Nombre descriptivo del catálogo
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Descripción detallada del propósito del catálogo
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Colección de elementos que pertenecen a este catálogo
        /// </summary>
        [JsonPropertyName("items")]
        public List<CatalogItem> Items { get; set; } = new List<CatalogItem>();
    }
}
