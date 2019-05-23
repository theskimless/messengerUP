using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationTest1.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Логин обязателен к заполнению")]
        public string Login { get; set; }

        [Required(ErrorMessage = "E-mail обязателен к заполнению")]
        [EmailAddress]
        [DataType(DataType.EmailAddress, ErrorMessage = "asdads")]
        public string Email{ get; set; }

        [Required(ErrorMessage = "Пароль обязателен к заполнению")]
        [StringLength(1024, MinimumLength = 6, ErrorMessage = "Минимальная длина пароля 6 символов")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[0-9])\S{6,128}$", ErrorMessage = "Неверный формат пароля. Пароль должен содержать хотя бы одну заглавную букву и цифру")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпали")]
        public string ConfirmPassword { get; set; }
    }
}
