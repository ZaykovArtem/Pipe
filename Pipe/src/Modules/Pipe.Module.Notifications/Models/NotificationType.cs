using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Notifications.Models
{
	public enum NotificationType
	{
		SystemAlert = 1,    // Системные уведомления

		UserCreated = 2,  // Поьзователь создан
		UserUpdated = 3,   // Поьзователь изменён 
		UserDeleted = 4,	// Поьзователь удален 

		NewMessage = 1000    // Новое сообщение
	}
}
