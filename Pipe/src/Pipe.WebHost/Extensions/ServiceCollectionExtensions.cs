using Microsoft.EntityFrameworkCore;
using Pipe.Infrastructure;
using Pipe.Infrastructure.Modules;
using Pipe.Module.Core.Data;
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
	}
}
