using System.Text.Json.Serialization;

namespace Core.Entities
{
    /// <summary>
    /// Entidad que representa un catálogo con su información básica y lista de elementos.
    /// </summary>
    public class Catalog
    {
        /// <summary>
        /// Identificador único del catálogo.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Nombre del catálogo.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Descripción del catálogo.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// Lista de elementos que pertenecen a este catálogo.
        /// </summary>
        [JsonPropertyName("items")]
        public List<CatalogItem> Items { get; set; } = new List<CatalogItem>();
    }
}
