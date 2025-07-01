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
	[Route("api/roles")]
	[Produces("application/json")]
	public class RoleApiController : ControllerBase
	{
		private readonly IRoleService _roleService;
		private readonly ILogger<RoleApiController> _logger;

		public RoleApiController(
			IRoleService roleService,
			ILogger<RoleApiController> logger)
		{
			_roleService = roleService;
			_logger = logger;
		}

		/// <summary>
		/// Получение списка роей 
		/// </summary>
		[HttpGet("grid")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> List()
		{
			try
			{
				var result = await _roleService.ListAsync();

				return result.IsSuccess
					? Ok(result.Value)
					: BadRequest(result.Message);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving role list");
				return StatusCode(500, "Internal server error");
			}
		}

	}

}
