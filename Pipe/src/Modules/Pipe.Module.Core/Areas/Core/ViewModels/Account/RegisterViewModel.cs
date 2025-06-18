using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Areas.Core.ViewModels.Account
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "Электронная почта обязательна")]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Ваше имя")]
		[Display(Name = "Имя")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Придумайте пароль")]
		[StringLength(100, ErrorMessage = "Пароль от 4рех до 100 символов", MinimumLength = 4)]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Повторите пароль")]
		[Compare("Password", ErrorMessage = "Пароли не совпадают(")]
		public string ConfirmPassword { get; set; }

		public string CallbackUrl { get; set; }
	}
}
