using System.ComponentModel.DataAnnotations;

namespace MyScatterPlotApp.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email является обязательным полем.")]
        [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль является обязательным полем.")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}
