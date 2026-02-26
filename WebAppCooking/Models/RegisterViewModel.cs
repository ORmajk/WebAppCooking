using System.ComponentModel.DataAnnotations;

namespace WebAppCooking.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Логин должен быть от 3 до 50 символов")]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Пароль должен быть от 4 до 50 символов")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Введите имя")]
        [StringLength(50)]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Введите возраст")]
        [Range(18, 120, ErrorMessage = "Возраст должен быть от 18 до 120")]
        [Display(Name = "Возраст")]
        public int Age { get; set; }
    }
}