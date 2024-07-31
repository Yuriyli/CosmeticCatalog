namespace CosmeticCatalog.Models
{
    /// <summary>
    /// Продукт, товар, изделие
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public Category? Category { get; set; }
        public List<Component> Components { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
        public List<ProductModification> Modifications { get; set; } = new();
    }
}
