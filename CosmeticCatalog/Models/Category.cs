namespace CosmeticCatalog.Models
{
    /// <summary>
    /// Категория продукта для построения дерева каталога
    /// </summary>
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? ParentId { get; set; }
        public Category? Parent { get; set; }
        public List<Category> Children { get; set; } = new();
        public List<Product> Products { get; set; } = new();
        public List<CategoryModification> Modifications { get; set; } = new();
    }
}
