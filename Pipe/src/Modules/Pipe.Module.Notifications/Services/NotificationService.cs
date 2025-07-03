using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pipe.Infrastructure.Data;
using Pipe.Infrastructure.Helpers;
using Pipe.Module.Notifications.Hubs;
using Pipe.Module.Notifications.Models;
using Pipe.Module.Notifications.Services;

public class NotificationService : INotificationService, IAsyncDisposable
{
	private readonly IRepository<Notification> _notificationRepository;
	private readonly IHubContext<NotificationHub> _hubContext;
	private readonly ILogger<NotificationService> _logger;
	private HubConnection? _hubConnection;

	public NotificationService(
		IRepository<Notification> notificationRepository,
		IHubContext<NotificationHub> hubContext,
		ILogger<NotificationService> logger)
	{
		_notificationRepository = notificationRepository;
		_hubContext = hubContext;
		_logger = logger;
	}

	/// <summary>
	/// Отправка нового уведомления
	/// </summary>
	public async Task<Result<Guid>> SendNotificationAsync(Notification notification)
	{
		try
		{
			notification.Id = Guid.NewGuid();
			notification.CreatedAt = DateTime.UtcNow;
			notification.IsRead = false;

			await _notificationRepository.AddAsync(notification);
			await _notificationRepository.SaveChangesAsync();

			await _hubContext.Clients
				.Group($"user-{notification.UserId}")
				.SendAsync("ReceiveNotification", notification);

			_logger.LogInformation("Notification sent to user {UserId}", notification.UserId);
			return Result<Guid>.Success(notification.Id);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error sending notification");
			return Result<Guid>.Failure("Failed to send notification");
		}
	}

	/// <summary>
	/// Получение уведомлений пользователя
	/// </summary>
	public async Task<Result<List<Notification>>> GetUserNotificationsAsync(Guid userId, int limit = 10)
	{
		try
		{
			var notifications = await _notificationRepository.Query()
				.Where(n => n.UserId == userId)
				.OrderByDescending(n => n.CreatedAt)
				.Take(limit)
				.ToListAsync();

			return Result<List<Notification>>.Success(notifications);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting notifications for user {UserId}", userId);
			return Result<List<Notification>>.Failure("Failed to get notifications");
		}
	}

	/// <summary>
	/// Пометка уведомления как прочитанного
	/// </summary>
	public async Task<Result<bool>> MarkAsReadAsync(Guid notificationId)
	{
		try
		{
			var notification = await _notificationRepository.Query().FirstOrDefaultAsync(x=>x.Id==notificationId);
			if (notification == null)
				return Result<bool>.NotFound("Notification not found");

			if (!notification.IsRead)
			{
				notification.IsRead = true;
				await _notificationRepository.SaveChangesAsync();
			}

			return Result<bool>.Success(true);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
			return Result<bool>.Failure("Failed to mark notification as read");
		}
	}

	/// <summary>
	/// Получение количества непрочитанных уведомлений
	/// </summary>
	public async Task<Result<int>> GetUnreadCountAsync(Guid userId)
	{
		try
		{
			var count = await _notificationRepository.Query()
				.CountAsync(n => n.UserId == userId && !n.IsRead);

			return Result<int>.Success(count);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error getting unread count for user {UserId}", userId);
			return Result<int>.Failure("Failed to get unread count");
		}
	}

	/// <summary>
	/// Подключение к SignalR Hub (для клиентских приложений)
	/// </summary>
	public async Task ConnectAsync(Guid userId)
	{
		_hubConnection = new HubConnectionBuilder()
			.WithUrl("/notificationHub")
			.WithAutomaticReconnect()
			.Build();

		await _hubConnection.StartAsync();
		await _hubConnection.InvokeAsync("Subscribe", userId);
	}

	public async ValueTask DisposeAsync()
	{
		if (_hubConnection != null)
		{
			await _hubConnection.DisposeAsync();
		}
	}
}