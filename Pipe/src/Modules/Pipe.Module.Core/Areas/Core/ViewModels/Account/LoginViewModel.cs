using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Areas.Core.ViewModels.Account
{
	public class LoginViewModel
	{
		[Required(ErrorMessage = "Электронная почта обязательна")]
		[EmailAddress(ErrorMessage = "Введите корректный адресс электронной почты")]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Придумайте пароль")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Display(Name = "Запомнить?")]
		public bool RememberMe { get; set; } = true;
	}
}
