using Microsoft.AspNetCore.Identity;
using Pipe.Infrastructure.Models;
using System;
using System.Collections.Generic;

namespace Pipe.Module.Core.Models
{
	public class User : IdentityUser<Guid>, IEntityWithTypedId<Guid>, IExtendableObject
	{
		public User()
		{
		}
		public IList<UserRole> Roles { get; set; } = new List<UserRole>();
		public string ExtensionData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
