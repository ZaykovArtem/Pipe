using Microsoft.AspNetCore.Mvc;
using Pipe.Infrastructure.Helpers;
using Pipe.Infrastructure.SmartTable;
using Pipe.Module.Core.Areas.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Services
{
	public interface IUserService
	{
		Task<Result<SmartTableResult<UserForm>>> ListAsync([FromBody] SmartTableParam param);
		Task<Result<UserForm>> GetAsync(Guid id);
		Task<Result<Guid>> CreateAsync(UserForm model);
		Task<Result> UpdateAsync(UserForm model);
		Task<Result> DeleteAsync(Guid model);
		Task<Result<List<string>>> GetUniqueRolesNameAsync();
	}
}
