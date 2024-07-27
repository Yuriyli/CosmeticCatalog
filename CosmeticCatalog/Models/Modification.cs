namespace CosmeticCatalog.Models
{
    public enum ModificationType
    {
        Create,
        Update,
        Delete
    }

    /// <summary>
    /// Для хранения истории изменений и отслеживания действий модераторов
    /// </summary>
    public class Modification
    {
        public int Id { get; set; }
        public required AppUser AppUser { get; set; }
        public required DateTime DateTime { get; set; }
        public ModificationType ModificationType { get; set; }
        public string? Info { get; set; }
    }

    // Для всех отслеживаемых моделей свой спец класс унаследованный от Modification    

    public class CategoryModification : Modification
    {
        public required Category Category { get; set; }
    }    
    public class ComponentModification : Modification
    {
        public required Component Component { get; set; }
    }    
    public class ProductModification : Modification
    {
        public required Product Product { get; set; }
    }    
    public class TagModification : Modification
    {
        public required Tag Tag { get; set; }
    }
    
}
