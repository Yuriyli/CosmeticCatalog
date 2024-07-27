namespace CosmeticCatalog.Models
{
    /// <summary>
    /// Компонент входящий в состав продукта
    /// </summary>
    public class Component
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<Tag> Tags { get; set; } = new ();
        public List<ComponentModification> Modifications { get; set; } = new();
    }
}
