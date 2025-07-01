using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pipe.Infrastructure.Data;
using Pipe.Infrastructure.Extensions;
using Pipe.Infrastructure.Helpers;
using Pipe.Infrastructure.SmartTable;
using Pipe.Module.Core.Areas.Core.ViewModels;
using Pipe.Module.Core.Models;
using Pipe.Module.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

public class UserService : IUserService
{
	private readonly IRepository<User> _userRepository;
	private readonly UserManager<User> _userManager;
	private readonly ILogger<UserService> _logger;

	public UserService(
		IRepository<User> userRepository,
		UserManager<User> userManager,
		ILogger<UserService> logger)
	{
		_userRepository = userRepository;
		_userManager = userManager;
		_logger = logger;
	}


	public async Task<Result<SmartTableResult<UserForm>>> ListAsync(SmartTableParam param)
	{
		try
		{
			var query = _userRepository.Query()
				.Include(x => x.Roles)
					.ThenInclude(x => x.Role)
				.Where(x => !x.IsDeleted);
			if (param.Search?.PredicateObject != null && param.Search.PredicateObject.Any())
			{
				if (param.Search.PredicateObject.TryGetValue("name", out var nameValue))
				{
					string nameSearch = nameValue?.ToString(); // Явное преобразование в строку
					if (!string.IsNullOrWhiteSpace(nameSearch))
					{
						string searchTerm = nameSearch.ToLower();
						query = query.Where(x =>
							x.Email.ToLower().Contains(searchTerm) ||
							x.FullName.ToLower().Contains(searchTerm));
					}
				}
				if (param.Search.PredicateObject.TryGetValue("roles", out var rolesValue))
				{
					var roleNames = JsonSerializer.Deserialize<string[]>(
					JsonSerializer.Serialize(rolesValue)) ?? Array.Empty<string>();

					if (roleNames.Length > 0)
					{
						query = query.Where(x => x.Roles.Any(ur =>
							roleNames.Contains(ur.Role.Name)));
					}
				}
			}
			var users = await query.ToSmartTableResultNoProjectionAsync(
			param,
			user => new UserForm()
			{
				Id = user.Id,
				Email = user.Email,
				FullName = user.FullName,
				CreatedOn = user.CreatedOn,
				LatestUpdatedOn = user.LatestUpdatedOn,
				RoleNames = user.Roles.Select(x => x.Role.Name),
			});

			return Result<SmartTableResult<UserForm>>.Success(users);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error while getting user list");
			return Result<SmartTableResult<UserForm>>.Failure("Failed to retrieve users");
		}
	}

	public async Task<Result<UserForm>> GetAsync(Guid id)
	{
		try
		{
			var user = await _userRepository.Query()
				.Include(x => x.Roles)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (user == null)
				return Result<UserForm>.NotFound("User not found");

			var model = new UserForm
			{
				Id = user.Id,
				FullName = user.FullName,
				LatestUpdatedOn = user.LatestUpdatedOn,
				CreatedOn = user.CreatedOn,
				Email = user.Email,
				RoleIds = user.Roles.Select(x => x.RoleId).ToList()
			};

			return Result<UserForm>.Success(model);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Error while getting user with ID {id}");
			return Result<UserForm>.Failure("Failed to retrieve user");
		}
	}

	public async Task<Result<Guid>> CreateAsync(UserForm model)
	{
		try
		{
			if (model == null)
				return Result<Guid>.Failure("User data is required");

			var user = new User
			{
				FullName = model.FullName,
				UserName = model.Email,
				Email = model.Email,
				CreatedOn = DateTimeOffset.Now,
				LatestUpdatedOn = DateTimeOffset.Now
			};

			foreach (var roleId in model.RoleIds)
			{
				user.Roles.Add(new UserRole { RoleId = roleId });
			}

			var creationResult = await _userManager.CreateAsync(user, model.Password);

			if (!creationResult.Succeeded)
			{
				var errors = string.Join(", ", creationResult.Errors.Select(e => e.Description));
				_logger.LogWarning($"User creation failed: {errors}");
				return Result<Guid>.Failure($"User creation failed: {errors}");
			}

			return Result<Guid>.Success(user.Id, "User created successfully");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error while creating user");
			return Result<Guid>.Failure("Failed to create user");
		}
	}

	public async Task<Result> UpdateAsync(UserForm model)
	{
		try
		{
			var user = await _userRepository.Query()
				.Include(x => x.Roles)
				.FirstOrDefaultAsync(x => x.Id == model.Id);

			if (user == null)
				return Result.NotFound("User not found");

			user.Email = model.Email;
			user.UserName = model.Email;
			user.FullName = model.FullName;
			user.LatestUpdatedOn = DateTimeOffset.Now;

			AddOrDeleteRoles(model, user);

			if (!string.IsNullOrEmpty(model.Password))
			{
				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.Password);

				if (!passwordResult.Succeeded)
				{
					var errors = string.Join(", ", passwordResult.Errors.Select(e => e.Description));
					_logger.LogWarning($"Password reset failed: {errors}");
					return Result.Failure($"Password update failed: {errors}");
				}
			}

			var updateResult = await _userManager.UpdateAsync(user);

			if (!updateResult.Succeeded)
			{
				var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
				_logger.LogWarning($"User update failed: {errors}");
				return Result.Failure($"User update failed: {errors}");
			}

			return Result.Success("User updated successfully");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Error while updating user with ID {model?.Id}");
			return Result.Failure("Failed to update user");
		}
	}


	public async Task<Result> DeleteAsync(Guid id)
	{
		try
		{
			var user = await _userRepository.Query()
				.FirstOrDefaultAsync(x => x.Id == id);

			if (user == null)
				return Result.NotFound("Пользователь не найден");

			await SoftDeleteUser(user);

			return Result.Success("Пользователь успешно удален");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Ошибка при удалении пользователя {UserId}", id);
			return Result.Failure("Не удалось удалить пользователя");
		}
	}

	private async Task SoftDeleteUser(User user)
	{
		user.IsDeleted = true;
		user.LockoutEnabled = true;
		user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100); // Более реалистичный срок блокировки
		await _userRepository.SaveChangesAsync();
	}
	public async Task<Result<List<string>>> GetUniqueRolesNameAsync()
	{
		try
		{
			var roles = await _userRepository.Query()
			.SelectMany(u => u.Roles.Select(r => r.Role.Name))
			.Distinct()
			.ToListAsync();
			if (roles == null)
				return Result<List<string>>.NotFound("Roles not found");
			return Result<List<string>>.Success(roles);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, $"Error while getting roles");
			return Result<List<string>>.Failure("Failed to getting roles");
		}
	}
	private void AddOrDeleteRoles(UserForm model, User user)
	{
		// Добавление новых ролей
		foreach (var roleId in model.RoleIds)
		{
			if (!user.Roles.Any(x => x.RoleId == roleId))
			{
				user.Roles.Add(new UserRole { RoleId = roleId, User = user });
			}
		}

		// Удаление отсутствующих ролей
		var rolesToRemove = user.Roles
			.Where(userRole => !model.RoleIds.Contains(userRole.RoleId))
			.ToList();

		foreach (var role in rolesToRemove)
		{
			user.Roles.Remove(role);
		}
	}


}