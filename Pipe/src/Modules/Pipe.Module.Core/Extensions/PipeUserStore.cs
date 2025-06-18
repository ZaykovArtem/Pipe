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
	public class PipeUserStore : UserStore<User, Role, PipeDbContext, Guid, IdentityUserClaim<Guid>, UserRole,
	IdentityUserLogin<Guid>, IdentityUserToken<Guid>, IdentityRoleClaim<Guid>>
	{
		public PipeUserStore(PipeDbContext context, IdentityErrorDescriber describer) : base(context, describer)
		{
		}
	}
}
