using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pipe.Infrastructure.Data;
using Pipe.Module.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Data
{
	public class CoreCustomModelBuilder : ICustomModelBuilder
	{
		public void Build(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<AppSetting>().ToTable("Core_AppSetting");

			modelBuilder.Entity<User>()
				.ToTable("Core_User");

			modelBuilder.Entity<Role>()
				.ToTable("Core_Role");

			modelBuilder.Entity<IdentityUserClaim<Guid>>(b =>
			{
				b.HasKey(uc => uc.Id);
				b.ToTable("Core_UserClaim");
			});

			modelBuilder.Entity<IdentityRoleClaim<Guid>>(b =>
			{
				b.HasKey(rc => rc.Id);
				b.ToTable("Core_RoleClaim");
			});

			modelBuilder.Entity<UserRole>(b =>
			{
				b.HasKey(ur => new { ur.UserId, ur.RoleId });
				b.HasOne(ur => ur.Role).WithMany(x => x.Users).HasForeignKey(r => r.RoleId);
				b.HasOne(ur => ur.User).WithMany(x => x.Roles).HasForeignKey(u => u.UserId);
				b.ToTable("Core_UserRole");
			});

			modelBuilder.Entity<IdentityUserLogin<Guid>>(b =>
			{
				b.ToTable("Core_UserLogin");
			});

			modelBuilder.Entity<IdentityUserToken<Guid>>(b =>
			{
				b.ToTable("Core_UserToken");
			});

			CoreSeedData.SeedData(modelBuilder);
		}
	}
}
