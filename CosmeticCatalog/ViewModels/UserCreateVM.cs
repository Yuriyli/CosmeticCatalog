#nullable disable

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CosmeticCatalog.ViewModels
{
    public class UserCreateVM
    {
        [Required]
        [MinLength(4)]
        [Display(Name = "Имя Учетной записи")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "{0} должен содержать {2} или максимум {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли должны совпадать.")]
        public string ConfirmPassword { get; set; }
    }
}
