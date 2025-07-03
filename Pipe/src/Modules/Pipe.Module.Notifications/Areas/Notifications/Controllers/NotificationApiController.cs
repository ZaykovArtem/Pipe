using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pipe.Module.Core.Models;
using Pipe.Module.Notifications.Models;
using Pipe.Module.Notifications.Services;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
	private readonly INotificationService _notificationService;
	private readonly UserManager<User> _userManager;
	private readonly ILogger<NotificationsController> _logger;

	public NotificationsController(
		INotificationService notificationService,
		UserManager<User> userManager,
		ILogger<NotificationsController> logger)
	{
		_notificationService = notificationService;
		_userManager = userManager;
		_logger = logger;
	}
	private async Task<Guid> GetCurrentUserAsync()
	{
		var user = await _userManager.GetUserAsync(HttpContext.User);
		return user.Id;
	}
	/// <summary>
	/// Получить уведомления текущего пользователя
	/// </summary>
	[HttpGet("grid")]
	[ProducesResponseType(typeof(List<Notification>), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> GetNotifications([FromQuery] int limit = 10)
	{
		var userId = await GetCurrentUserAsync();
		var result = await _notificationService.GetUserNotificationsAsync(userId, limit);

		return result.IsSuccess
		? Ok(result.Value)
		: BadRequest(result.Message);
	}

	/// <summary>
	/// Получить количество непрочитанных уведомлений
	/// </summary>
	[HttpGet("unread/count")]
	[ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetUnreadCount()
	{
		var userId = await GetCurrentUserAsync();
		var result = await _notificationService.GetUnreadCountAsync(userId);

		return result.IsSuccess
		? Ok(result.Value)
		: BadRequest(result.Message);
	}

	/// <summary>
	/// Пометить уведомление как прочитанное
	/// </summary>
	[HttpPatch("{id}/read")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> MarkAsRead(Guid id)
	{
		var result = await _notificationService.MarkAsReadAsync(id);

		if (result.IsSuccess && result.Value)
		{
			_logger.LogInformation("Notification {NotificationId} marked as read", id);
		}

		return result.IsSuccess
		? Ok(result.Value)
		: BadRequest(result.Message);
	}

	/// <summary>
	/// Создать тестовое уведомление (для админов)
	/// </summary>
	[HttpPost("test")]
	[Authorize(Roles = "Admin")]
	[ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
	public async Task<IActionResult> CreateTestNotification([FromBody] TestNotificationRequest request)
	{
		var notification = new Notification
		{
			UserId = request.UserId,
			Title = request.Title,
			Message = request.Message,
			Type = request.Type,
			Url = request.Url
		};

		var result = await _notificationService.SendNotificationAsync(notification);
		return result.IsSuccess
		? Ok(result.Value)
		: BadRequest(result.Message); 
	}
}

public record TestNotificationRequest(
	Guid UserId,
	string Title,
	string Message,
	NotificationType Type,
	string? Url = null);