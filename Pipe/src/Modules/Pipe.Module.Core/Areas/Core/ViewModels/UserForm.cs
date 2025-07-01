using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Areas.Core.ViewModels
{
	public class UserForm
	{
		public Guid Id { get; set; }
		[Required(ErrorMessage = "Имя обязательно")]
		[StringLength(30, ErrorMessage = "Имя не может быть короче 3-рех символов и длиннее 30", MinimumLength = 3)]
		public string FullName { get; set; }

		[Required(ErrorMessage = "Почта обязательна")]
		[EmailAddress(ErrorMessage = "Введите реальный Email адрес")]
		public string Email { get; set; }

		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset LatestUpdatedOn { get; set; }
		public IList<Guid> RoleIds { get; set; } = new List<Guid>();
		public IEnumerable<string> RoleNames { get; set; }

		[DataType(DataType.Password)]
		[StringLength(100, ErrorMessage = "Не более {1} и не менее {2} сиволов.", MinimumLength = 4)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
		public string? ConfirmPassword { get; set; }
	}
}
