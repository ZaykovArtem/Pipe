using Microsoft.AspNetCore.Identity;
using Pipe.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pipe.Module.Core.Models
{
	public class User : IdentityUser<Guid>, IEntityWithTypedId<Guid>, IExtendableObject
	{
		public User()
		{
		}
		[MaxLength(450)]
		public string FullName { get; set; }
		public IList<UserRole> Roles { get; set; } = new List<UserRole>();
		public string ExtensionData { get; set; }
	}
}
