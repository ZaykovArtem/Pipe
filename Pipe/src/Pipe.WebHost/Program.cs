using Microsoft.EntityFrameworkCore;
using Pipe.Infrastructure;
using Pipe.Infrastructure.Data;
using Pipe.Infrastructure.Modules;
using Pipe.Module.Core.Data;
using Pipe.Module.Core.Extensions;
using Pipe.WebHost.Extensions;
using System.Data;

var builder = WebApplication.CreateBuilder(args);
ConfigureService();
var app = builder.Build();
Configure();
app.Run();

void ConfigureService()
{

	var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

	builder.Configuration.AddEntityFrameworkConfig(options =>
	{
		options.UseNpgsql(connectionString);
	});

	GlobalConfiguration.WebRootPath = builder.Environment.WebRootPath;
	GlobalConfiguration.ContentRootPath = builder.Environment.ContentRootPath;

	builder.Services.AddModules();
	builder.Services.AddCustomizedDataStore(builder.Configuration);
	builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
	builder.Services.AddTransient(typeof(IRepositoryWithTypedId<,>), typeof(RepositoryWithTypedId<,>));
	builder.Services.AddRazorPages();
	builder.Services.AddMvc();
	foreach (var module in GlobalConfiguration.Modules)
	{
		var moduleInitializerType = module.Assembly.GetTypes()
		   .FirstOrDefault(t => typeof(IModuleInitializer).IsAssignableFrom(t));
		if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer)))
		{
			var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
			builder.Services.AddSingleton(typeof(IModuleInitializer), moduleInitializer);
			moduleInitializer.ConfigureServices(builder.Services);
		}
	}

}
void Configure()
{
	if (app.Environment.IsDevelopment())
	{
		app.UseDeveloperExceptionPage();
		app.UseMigrationsEndPoint();
	}
	app.UseStaticFiles();
	app.UseRouting();
	app.UseEndpoints(endpoints =>
	{
		endpoints.MapControllerRoute(
			name: "areas",
			pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
		endpoints.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");
	});
	var moduleInitializers = app.Services.GetServices<IModuleInitializer>();
	foreach (var moduleInitializer in moduleInitializers)
	{
		moduleInitializer.Configure(app, builder.Environment);
	}
}