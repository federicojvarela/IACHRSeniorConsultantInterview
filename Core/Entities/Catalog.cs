using System.Text.Json.Serialization;

namespace Core.Entities
{
    public class Catalog
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("items")]
        public List<CatalogItem> Items { get; set; } = new List<CatalogItem>();
    }
}
