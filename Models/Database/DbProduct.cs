using System.ComponentModel.DataAnnotations;

namespace AltenShopApi.Models.Database
{
    public class DbProduct
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public int Price { get; set; }
        //public int Quantity { get; set; }
        public string InternalReference { get; set; }
        public int ShellId { get; set; }
        public string InventoryStatus { get; set; } // --> ENUM "INSTOCK" | "LOWSTOCK" | "OUTOFSTOCK";
        public int Rating { get; set; }
        public long CreatedAt { get; set; }
        public long UpdatedAt { get; set; }
    }
}
