using MediatR;
using Microsoft.Extensions.Logging;
using Pipe.Infrastructure.Helpers;
using Pipe.Module.Core.Events;
using Pipe.Module.Notifications.Models;
using Pipe.Module.Notifications.Services;

public class NotificationHandler :
	INotificationHandler<UserCreatedEvent>,
	INotificationHandler<UserUpdatedEvent>,
	INotificationHandler<UserDeletedEvent>
{
	private readonly INotificationService _notificationService;
	private readonly ILogger<NotificationHandler> _logger;

	public NotificationHandler(
		INotificationService notificationService,
		ILogger<NotificationHandler> logger)
	{
		_notificationService = notificationService;
		_logger = logger;
	}

	public async Task Handle(UserCreatedEvent notificationEvent, CancellationToken cancellationToken)
	{
		var result = await _notificationService.SendNotificationAsync(new Notification
		{
			UserId = notificationEvent.UserId,
			Title = "Новый пользователь в системе",
			Message = $"Зарегистрирован новый пользователь: {notificationEvent.Username}",
			Type = (NotificationType)notificationEvent.Type,
			Url = notificationEvent.Url,
			DataJson = notificationEvent.DataJson,
			CreatedAt = DateTime.UtcNow,
			IsRead = false
		});

		if (result.IsSuccess)
		{
			_logger.LogInformation(
				"Уведомление о создании пользователя отправлено. ID: {NotificationId}, Пользователь: {Username}",
				result.Value, notificationEvent.Username);
		}
		else
		{
			_logger.LogError(
				"Ошибка при отправке уведомления о создании пользователя {Username}. Ошибка: {Error}",
				notificationEvent.Username, result.Message);
		}
	}

	// Аналогичные реализации для других типов событий
	public async Task Handle(UserUpdatedEvent notificationEvent, CancellationToken cancellationToken)
	{
		var result = await _notificationService.SendNotificationAsync(new Notification
		{
			UserId = notificationEvent.UserId,
			Title = "Обновление профиля",
			Message = $"Изменены данные пользователя {notificationEvent.Username}",
			Type = (NotificationType)notificationEvent.Type,
			Url = notificationEvent.Url,
			DataJson = notificationEvent.DataJson
		});

		LogNotificationResult(result, "UserUpdated", notificationEvent);
	}

	public async Task Handle(UserDeletedEvent notificationEvent, CancellationToken cancellationToken)
	{
		var result = await _notificationService.SendNotificationAsync(new Notification
		{
			UserId = notificationEvent.UserId,
			Title = "Удаление пользователя",
			Message = $"Пользователь {notificationEvent.Username} был удалён",
			Type = (NotificationType)notificationEvent.Type,
			Url = notificationEvent.Url,
			DataJson = notificationEvent.DataJson
		});

		LogNotificationResult(result, "UserDeleted", notificationEvent);
	}

	private void LogNotificationResult<T>(Result<Guid> result, string eventType, T eventData) where T : INotificationEvent
	{
		if (result.IsSuccess)
		{
			_logger.LogInformation(
				"{EventType} notification sent successfully. NotificationId: {NotificationId}, TargetUrl: {Url}",
				eventType, result.Value, eventData.Url);
		}
		else
		{
			_logger.LogError(
				"Failed to send {EventType} notification. UserId: {UserId}, Error: {Error}",
				eventType, eventData.UserId, result.Message);
		}
	}
}