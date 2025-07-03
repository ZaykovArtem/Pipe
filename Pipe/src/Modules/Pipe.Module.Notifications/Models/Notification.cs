using Pipe.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pipe.Module.Notifications.Models
{
	public class Notification: EntityBaseWithTypedId<Guid>
	{
		public Guid Id { get; set; }
		public required Guid UserId { get; set; }
		public required string Title { get; set; }
		public required string Message { get; set; }
		public bool IsRead { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
		public NotificationType Type { get; set; }
		public string? Url { get; set; } // Ссылка для действия
		public string? DataJson { get; set; } // Доп. данные в JSON

		public T? GetData<T>() => DataJson != null
			? JsonSerializer.Deserialize<T>(DataJson)
			: default;
	}
}
