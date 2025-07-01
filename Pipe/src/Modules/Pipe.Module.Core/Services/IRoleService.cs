using Pipe.Infrastructure.Helpers;
using Pipe.Module.Core.Areas.Core.ViewModels;
using Pipe.Module.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Services
{
	public interface IRoleService
	{
		public Task<Result<List<RoleForm>>> ListAsync();
	}
}
