using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pipe.Infrastructure;
using Pipe.Infrastructure.Modules;
using Pipe.Module.Core.Data;
using Pipe.Module.Core.Extensions;
using Pipe.Module.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;

namespace Pipe.WebHost.Extensions
{
	public static class ServiceCollectionExtensions
	{
		private static readonly IModuleConfigurationManager _modulesConfig = new ModuleConfigurationManager();

		public static IServiceCollection AddModules(this IServiceCollection services)
		{
			foreach (var module in _modulesConfig.GetModules())
			{
				module.Assembly = Assembly.Load(new AssemblyName(module.Id));
				GlobalConfiguration.Modules.Add(module);
			}
			return services;
		}
		public static IServiceCollection AddCustomizedDataStore(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContextPool<PipeDbContext>(options =>
			{
				options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Pipe.WebHost"));
				options.EnableSensitiveDataLogging();
			});
			return services;
		}
		public static IServiceCollection AddCustomizedIdentity(this IServiceCollection services, IConfiguration configuration)
		{
			services
				.AddIdentity<User, Role>(options =>
				{
					options.Password.RequireDigit = false;
					options.Password.RequiredLength = 4;
					options.Password.RequireNonAlphanumeric = false;
					options.Password.RequireUppercase = false;
					options.Password.RequireLowercase = false;
					options.Password.RequiredUniqueChars = 0;
					options.ClaimsIdentity.UserNameClaimType = JwtRegisteredClaimNames.Sub;
				})
				.AddRoleStore<PipeRoleStore>()
				.AddUserStore<PipeUserStore>()
				.AddSignInManager<PipeSignInManager<User>>()
				.AddDefaultTokenProviders();



			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie();

			services.ConfigureApplicationCookie(x =>
			{
				x.LoginPath = new PathString("/login");
			});
			return services;
		}
	}
}
