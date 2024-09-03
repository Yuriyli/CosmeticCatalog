using CosmeticCatalog.Models;
using System.ComponentModel.DataAnnotations;

namespace CosmeticCatalog.ViewModels
{
    public class CategoryEditVM
    {
        public int Id { get; set; }

        [Display(Name = "Название категории")]
        [Required(ErrorMessage = "Требуется название категории")]
        [MaxLength(32, ErrorMessage = "Максимальная длина 32 символов")]
        [MinLength(3, ErrorMessage = "Минимальная длина 3 символа")]
        public required string Name { get; set; }

        public int? ParentId { get; set; }
    }
}
