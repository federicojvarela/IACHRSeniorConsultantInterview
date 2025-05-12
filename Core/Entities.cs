using System.Text.Json.Serialization;

namespace Core.Entities
{
    public class Document
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public DateTime UploadDate { get; set; }
        public ProcessingStatus Status { get; set; }
        public string ProcessingResult { get; set; }
    }

    public enum ProcessingStatus
    {
        Pending,
        Processing,
        Completed,
        Failed
    }

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

    public class CatalogItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
