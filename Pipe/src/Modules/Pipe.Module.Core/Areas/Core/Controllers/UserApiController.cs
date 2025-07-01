using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pipe.Infrastructure.Data;
using Pipe.Infrastructure.Helpers;
using Pipe.Infrastructure.SmartTable;
using Pipe.Module.Core.Areas.Core.ViewModels;
using Pipe.Module.Core.Models;
using Pipe.Module.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Areas.Core.Controllers
{
	[ApiController]
	[Area("Core")]
	[Authorize(Roles = "admin")]
	[Route("api/users")]
	[Produces("application/json")]
	public class UserApiController : ControllerBase
	{
		private readonly IUserService _userService;
		private readonly ILogger<UserApiController> _logger;

		public UserApiController(
			IUserService userService,
			ILogger<UserApiController> logger)
		{
			_userService = userService;
			_logger = logger;
		}

		/// <summary>
		/// Получение списка пользователей с пагинацией
		/// </summary>
		[HttpPost("grid")]
		[ProducesResponseType(typeof(SmartTableResult<UserForm>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> List([FromBody] SmartTableParam param)
		{
			try
			{
				var result = await _userService.ListAsync(param);

				return result.IsSuccess
					? Ok(result.Value)
					: BadRequest(result.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving user list");
				return StatusCode(500, "Internal server error");
			}
		}

		/// <summary>
		/// Получение данных пользователя
		/// </summary>
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(UserForm), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Get(Guid id)
		{
			try
			{
				var result = await _userService.GetAsync(id);

				if (result.IsNotFound)
					return NotFound(result.Message);

				return result.IsSuccess
					? Ok(result.Value)
					: BadRequest(result.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error retrieving user with ID {id}");
				return StatusCode(500, "Internal server error");
			}
		}

		/// <summary>
		/// Создание нового пользователя
		/// </summary>
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> Create([FromBody] UserForm model)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var result = await _userService.CreateAsync(model);

				if (!result.IsSuccess)
					return BadRequest(result.Message);

				return CreatedAtAction(nameof(Get), new { id = result.Value }, null);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating user");
				return StatusCode(500, "Internal server error");
			}
		}

		/// <summary>
		/// Обновление данных пользователя
		/// </summary>
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Update(Guid id, [FromBody] UserForm model)
		{
			try
			{
				if (id != model.Id)
					return BadRequest("ID mismatch");

				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var result = await _userService.UpdateAsync(model);

				if (result.IsNotFound)
					return NotFound(result.Message);

				return result.IsSuccess
					? NoContent()
					: BadRequest(result.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error updating user with ID {id}");
				return StatusCode(500, "Internal server error");
			}
		}

		/// <summary>
		/// Удаление пользователя
		/// </summary>
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Delete(Guid id)
		{
			try
			{
				var result = await _userService.DeleteAsync(id);

				if (result.IsNotFound)
					return NotFound(result.Message);

				return result.IsSuccess
					? NoContent()
					: BadRequest(result.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error deleting user with ID {id}");
				return StatusCode(500, "Internal server error");
			}
		}
		/// <summary>
		/// Получение уникальных ролей для фильтров
		/// </summary>
		/// <returns></returns>
		[HttpGet("roles")]
		[ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> GetUniqueRoles()
		{
			var result = await _userService.GetUniqueRolesNameAsync();

			return result.IsSuccess
					? Ok(result.Value)
					: BadRequest(result.Message);
		}
	}

}
