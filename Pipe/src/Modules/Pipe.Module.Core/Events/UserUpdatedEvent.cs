using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Events
{
	public class UserUpdatedEvent : INotificationEvent
	{
		public Guid UserId { get; set; } // ID пользователя, который создан
		public Guid TriggeredByUserId { get; set; } // Кто создал (опционально)
		public string Username { get; set; } // Доп. данные

		public int Type => 3;
		public string? Url => $"/users/{UserId}";
		public string? DataJson => JsonSerializer.Serialize(new { Username });
	}
}
