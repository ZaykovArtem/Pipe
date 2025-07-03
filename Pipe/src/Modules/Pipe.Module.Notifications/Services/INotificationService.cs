using Pipe.Infrastructure.Helpers;
using Pipe.Module.Notifications.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Notifications.Services
{
	public interface INotificationService
	{
		Task<Result<Guid>> SendNotificationAsync(Notification notification);
		Task<Result<List<Notification>>> GetUserNotificationsAsync(Guid userId, int limit = 10);
		Task<Result<bool>> MarkAsReadAsync(Guid notificationId);
		Task<Result<int>> GetUnreadCountAsync(Guid userId);
	}
}
