using Microsoft.AspNetCore.Identity;
using Pipe.Infrastructure.Models;
using System;
using System.Collections.Generic;

namespace Pipe.Module.Core.Models
{
	public class Role : IdentityRole<Guid>, IEntityWithTypedId<Guid>
	{
		public IList<UserRole> Users { get; set; } = new List<UserRole>();
	}
}
