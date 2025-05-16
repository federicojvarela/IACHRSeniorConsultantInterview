using System.Text.Json.Serialization;

namespace Core.Entities
{
    /// <summary>
    /// Entidad que representa un elemento individual dentro de un catálogo.
    /// </summary>
    public class CatalogItem
    {
        /// <summary>
        /// Identificador único del elemento del catálogo.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Nombre descriptivo del elemento del catálogo.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Valor asociado al elemento del catálogo (por ejemplo, content-type).
        /// </summary>
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
