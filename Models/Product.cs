using System.Text.Json.Serialization;

namespace AltenShopApi.Models
{
    public class Product
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("image")]
        public string Image { get; set; }
        [JsonPropertyName("category")]
        public string Category { get; set; }
        [JsonPropertyName("price")]
        public int Price { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonPropertyName("internalReference")]
        public string InternalReference { get; set; }
        [JsonPropertyName("shellId")]
        public int ShellId { get; set; }
        [JsonPropertyName("inventoryStatus")]
        public string InventoryStatus { get; set; } // --> ENUM "INSTOCK" | "LOWSTOCK" | "OUTOFSTOCK";
        [JsonPropertyName("rating")]
        public int Rating { get; set; }
        [JsonPropertyName("createdAt")]
        public long CreatedAt { get; set; }
        [JsonPropertyName("updatedAt")]
        public long UpdatedAt { get; set; }
    }
}
