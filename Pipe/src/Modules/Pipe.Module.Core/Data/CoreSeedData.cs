using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Pipe.Module.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Data
{
	public static class CoreSeedData
	{
		public static void SeedData(ModelBuilder builder)
		{
			builder.Entity<AppSetting>().HasData(
				new AppSetting("Global.AssetVersion") { Module = "Core", Value = "1.0" }
			);

			builder.Entity<Role>().HasData(
				new Role { Id = Guid.Parse("b26346f5-c8e7-446c-9184-9386595685fd"), ConcurrencyStamp = "f6e2ddef-7ff2-417a-a854-03f5718bc1ef", Name = "admin", NormalizedName = "ADMIN" },
				new Role { Id = Guid.Parse("3010a874-2a7a-42b9-92bd-67ae15d692c9"), ConcurrencyStamp = "8b522fc0-d6f7-4191-b286-80bb5ea4cbe9", Name = "customer", NormalizedName = "CUSTOMER" },
				new Role { Id = Guid.Parse("f73d14c2-dece-4eb0-837e-8c381bdab4a3"), ConcurrencyStamp = "c85f6961-bf4c-488f-8358-01b639e8139e", Name = "guest", NormalizedName = "GUEST" },
				new Role { Id = Guid.Parse("de4eb276-cc52-4478-a100-ad4f41a992c5"), ConcurrencyStamp = "a757379c-fe3c-4b8a-ab58-e8c8ef449668", Name = "vendor", NormalizedName = "VENDOR" }
			);

			builder.Entity<User>().HasData(
			);

			builder.Entity<UserRole>().HasData(
			);
		}
	}
}
