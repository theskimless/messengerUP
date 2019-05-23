using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationTest1.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Пароль обязателен к заполнению")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Логин обязателен к заполнению")]
        public string Password { get; set; }
    }
}
