using Microsoft.EntityFrameworkCore.Design;
using Pipe.Infrastructure;
using Pipe.Module.Core.Data;
using Pipe.WebHost.Extensions;

namespace Pipe.WebHost
{
	public class MigrationDbContextFactory : IDesignTimeDbContextFactory<PipeDbContext>
	{
		public PipeDbContext CreateDbContext(string[] args)
		{
			var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			var contentRootPath = Directory.GetCurrentDirectory();

			var builder = new ConfigurationBuilder()
							.SetBasePath(contentRootPath)
							.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
							.AddJsonFile($"appsettings.{environmentName}.json", true);

			builder.AddUserSecrets(typeof(MigrationDbContextFactory).Assembly, optional: true);
			builder.AddEnvironmentVariables();
			var _configuration = builder.Build();

			IServiceCollection services = new ServiceCollection();
			GlobalConfiguration.ContentRootPath = contentRootPath;
			services.AddModules();
			services.AddCustomizedDataStore(_configuration);
			var _serviceProvider = services.BuildServiceProvider();

			return _serviceProvider.GetRequiredService<PipeDbContext>();
		}
	}
}
