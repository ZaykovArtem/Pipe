using Microsoft.EntityFrameworkCore;
using Pipe.Infrastructure;
using Pipe.Infrastructure.Data;
using Pipe.Infrastructure.Modules;
using Pipe.Module.Core.Data;
using Pipe.Module.Core.Extensions;
using Pipe.WebHost.Extensions;

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
	builder.Services.AddCustomizedIdentity(builder.Configuration);
	builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
	builder.Services.AddTransient(typeof(IRepositoryWithTypedId<,>), typeof(RepositoryWithTypedId<,>));
	builder.Services.AddRazorPages();
	builder.Services.AddOutputCache();
	builder.Services.AddServerSideBlazor();
	builder.Services.AddMvc();
	builder.Services.AddAntiforgery(options =>
	{
		options.HeaderName = "X-XSRF-TOKEN"; // Äëÿ AJAX/SPA
		options.Cookie.Name = "XSRF-TOKEN";
		options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

	});
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
		app.UseExceptionHandler("/Error");
		app.UseStatusCodePagesWithReExecute("/Error/{0}");
		app.UseDeveloperExceptionPage();
		app.UseMigrationsEndPoint();
	}
	app.UseStaticFiles();
	app.UseRouting();
	app.UseAuthorization();
	app.UseEndpoints(endpoints =>
	{
		endpoints.MapControllerRoute(
			name: "areas",
			pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
		endpoints.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");
		endpoints.MapBlazorHub();
		endpoints.MapFallbackToPage("/admin/{**segment}", "/admin/_Host");
	});
	var moduleInitializers = app.Services.GetServices<IModuleInitializer>();
	foreach (var moduleInitializer in moduleInitializers)
	{
		moduleInitializer.Configure(app, builder.Environment);
	}
}