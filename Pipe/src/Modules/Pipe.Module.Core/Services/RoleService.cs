using Microsoft.EntityFrameworkCore;
using Pipe.Infrastructure.Data;
using Pipe.Infrastructure.Helpers;
using Pipe.Infrastructure.SmartTable;
using Pipe.Module.Core.Areas.Core.ViewModels;
using Pipe.Module.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Services
{
	public class RoleService : IRoleService
	{
		private readonly IRepository<Role> _roleRepository;
		public RoleService(IRepository<Role> roleRepository)
		{
			_roleRepository = roleRepository;
		}
		public async Task<Result<List<RoleForm>>> ListAsync()
		{
			var roles = await _roleRepository.Query().Select(x => new RoleForm()
			{
				Id = x.Id,
				Name = x.Name
			}).ToListAsync();

			if (roles == null)
			{
				throw new ArgumentException();
			}
			return Result<List<RoleForm>>.Success(roles);
		}
	}
}
