using CosmeticCatalog.Models;
using System.ComponentModel.DataAnnotations;

namespace CosmeticCatalog.ViewModels
{
    public class ComponentEditVM
    {
        public int Id { get; set; }

        [Display(Name = "Название компонента")]
        [Required(ErrorMessage = "Требуется название компонента")]
        public required string Name { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        public List<int> TagIds { get; set; } = new();
    }
}
