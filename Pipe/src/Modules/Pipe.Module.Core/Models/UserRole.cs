using Microsoft.AspNetCore.Identity;
using System;

namespace Pipe.Module.Core.Models
{
	public class UserRole : IdentityUserRole<Guid>
	{
		public override Guid UserId { get; set; }

		public User User { get; set; }

		public override Guid RoleId { get; set; }

		public Role Role { get; set; }
	}
}
