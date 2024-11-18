using CosmeticCatalog.Models;
using System.ComponentModel.DataAnnotations;

namespace CosmeticCatalog.ViewModels
{
    public class ProductEditVM
    {
        public int Id { get; set; }

        [Display(Name = "Название продукта")]
        [Required(ErrorMessage = "Требуется название продукта")]
        [MaxLength(32, ErrorMessage = "Максимальная длина 32 символов")]
        [MinLength(3, ErrorMessage = "Минимальная длина 3 символа")]
        public required string Name { get; set; }

        [Display(Name = "Описание продукта")]
        public string? Description { get; set; }

        // CateoryID - название изменено из за селектора категорий
        public int? ParentId { get; set; }

        public List<int> ComponentIds { get; set; } = new();

        public List<int> TagIds { get; set; } = new();
    }
}
