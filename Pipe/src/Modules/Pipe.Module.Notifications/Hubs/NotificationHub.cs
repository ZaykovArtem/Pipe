using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Pipe.Module.Notifications.Services;
using System.Security.Claims;
namespace Pipe.Module.Notifications.Hubs
{
	[Authorize]
	public class NotificationHub : Hub
	{
		private readonly INotificationService _notificationService;

		public NotificationHub(INotificationService notificationService)
		{
			_notificationService = notificationService;
		}
		public override async Task OnConnectedAsync()
		{
			var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId != null)
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
			}
			await base.OnConnectedAsync();
		}
		public async Task Subscribe(Guid userId)
		{
			await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");

			// При подключении отправляем последние непрочитанные уведомления
			var unreadNotifications = await _notificationService.GetUserNotificationsAsync(userId);
			foreach (var notification in unreadNotifications.Value.Where(n => !n.IsRead))
			{
				await Clients.Caller.SendAsync("ReceiveNotification", notification);
			}
		}
	}
}
