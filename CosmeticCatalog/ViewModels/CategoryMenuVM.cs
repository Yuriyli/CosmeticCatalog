using CosmeticCatalog.Models;

namespace CosmeticCatalog.ViewModels
{
    public class CategoryMenuVM
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? ParentId { get; set; }
        public bool HasChildren { get; set; }
        public bool HasProducts { get; set; }
        public bool IsOpen { get; set; }
        public bool IsActive { get; set; }
    }
}
