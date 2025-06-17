using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pipe.Infrastructure.Data;
using Pipe.Module.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.TestProject.Areas.TestProject.Controllers
{
	[Area("TestProject")]
	public class TestController : Controller
	{
		private readonly IRepositoryWithTypedId<AppSetting, string> _appSettingRepository;
		public TestController(IRepositoryWithTypedId<AppSetting, string> appSettingRepository)
		{
			_appSettingRepository = appSettingRepository;
		}

		[HttpGet("/test")]
		public async Task<IActionResult> Test()
		{
			var settings = await _appSettingRepository.Query().ToListAsync();
			return View(settings);
		}
	}
}
