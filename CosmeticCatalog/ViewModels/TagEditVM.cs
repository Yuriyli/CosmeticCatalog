using System.ComponentModel.DataAnnotations;

namespace CosmeticCatalog.ViewModels
{
    /// <summary>
    /// Теги для фронта может прикреплятся к продукту либо к компоненту из состава
    /// </summary>
    public class TagEditVM
    {
        public int Id { get; set; }

        [Display(Name = "Название тега")]
        [Required(ErrorMessage = "Требуется название тега")]
        [MaxLength(32, ErrorMessage = "Максимальная длина 32 символов")]
        [MinLength(3, ErrorMessage = "Минимальная длина 3 символа")]
        public required string Name { get; set; }

        public string? OriginalName { get; set; }

        public bool IsDeletable { get; set; }
    }

}
