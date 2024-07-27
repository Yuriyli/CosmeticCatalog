namespace CosmeticCatalog.Models
{
    /// <summary>
    /// Теги для фронта может прикреплятся к продукту либо к компоненту из состава
    /// </summary>
    public class Tag
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public TagType TagType { get; set; }
        public List<Product> Products { get; set; } = new();
        public List<Component> Components { get; set; } = new();
        public List<TagModification> Modifications { get; set; } = new();

    }

    /// <summary>
    /// Типы для внешнего вида тегов на фронте
    /// </summary>
    public enum TagType
    {
        Green,
        Red,
        Blue
    }
}
