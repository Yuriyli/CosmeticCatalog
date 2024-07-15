using System.ComponentModel.DataAnnotations;

namespace CosmeticCatalog.ViewModels
{
    public class LoginVM
    {
        [Required]
        [Display(Name = "Имя учетной записи")]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = null!;
            
        public string? ReturnUrl { get; set; }

        public bool Remember { get; set; }
    }
}
