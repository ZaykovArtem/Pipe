using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Pipe.Module.Core.Data;
using Pipe.Module.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Extensions
{
	public class PipeRoleStore : RoleStore<Role, PipeDbContext, Guid, UserRole, IdentityRoleClaim<Guid>>
	{
		public PipeRoleStore(PipeDbContext context) : base(context)
		{
		}
	}
}
