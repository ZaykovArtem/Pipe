using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Events
{
	public interface INotificationEvent : INotification
	{
		Guid UserId { get; } // Кому отправляем уведомление
		int Type { get; }
		string? Url { get; }
		string? DataJson { get; }
	}
}
