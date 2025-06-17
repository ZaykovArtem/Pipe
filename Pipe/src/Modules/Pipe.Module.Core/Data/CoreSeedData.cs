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
			);

			builder.Entity<User>().HasData(
			);

			builder.Entity<UserRole>().HasData(
			);
		}
	}
}
