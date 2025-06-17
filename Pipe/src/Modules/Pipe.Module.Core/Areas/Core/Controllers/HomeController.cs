using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Areas.Core.Controllers
{
	[Area("Core")]
	public class HomeController : Controller
	{
		public HomeController()
		{
		}

		[HttpGet("/")]
		public IActionResult Index()
		{
			return View();
		}
	}
}
