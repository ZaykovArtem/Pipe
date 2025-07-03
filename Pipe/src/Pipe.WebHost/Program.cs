using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pipe.Infrastructure;
using Pipe.Infrastructure.Data;
using Pipe.Infrastructure.Hendlers;
using Pipe.Infrastructure.Modules;
using Pipe.Module.Core.Data;
using Pipe.Module.Core.Extensions;
using Pipe.Module.Notifications.Hubs;
using Pipe.WebHost.Extensions;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация сервисов
ConfigureServices();

var app = builder.Build();

// Конфигурация middleware
ConfigureMiddleware();

app.Run();

void ConfigureServices()
{
	var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

	// Конфигурация БД
	builder.Configuration.AddEntityFrameworkConfig(options =>
	{
		options.UseNpgsql(connectionString, npgsqlOptions =>
		{
			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
		});
	});

	// Настройка путей
	GlobalConfiguration.WebRootPath = builder.Environment.WebRootPath;
	GlobalConfiguration.ContentRootPath = builder.Environment.ContentRootPath;

	// Добавление модулей
	builder.Services.AddModules();
	builder.Services.AddCustomizedDataStore(builder.Configuration);
	builder.Services.AddHttpContextAccessor();
	// Настройка Identity
	builder.Services.AddCustomizedIdentity(builder.Configuration);

	// Репозитории
	builder.Services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
	builder.Services.AddTransient(typeof(IRepositoryWithTypedId<,>), typeof(RepositoryWithTypedId<,>));

	// MVC и Blazor
	builder.Services.AddRazorPages();
	builder.Services.AddServerSideBlazor();
	builder.Services.AddControllersWithViews();

	// Кэширование
	builder.Services.AddOutputCache();

	// Защита от CSRF
	builder.Services.AddAntiforgery(options =>
	{
		options.HeaderName = "X-XSRF-TOKEN";
		options.Cookie.Name = "XSRF-TOKEN";
		options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
	});

	builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

	// Настройка кук для API
	builder.Services.ConfigureApplicationCookie(options =>
	{
		options.Events.OnRedirectToLogin = context =>
		{
			if (context.Request.Path.StartsWithSegments("/api"))
			{
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				return Task.CompletedTask;
			}
			context.Response.Redirect(context.RedirectUri);
			return Task.CompletedTask;
		};

		options.Events.OnRedirectToAccessDenied = context =>
		{
			if (context.Request.Path.StartsWithSegments("/api"))
			{
				context.Response.StatusCode = StatusCodes.Status403Forbidden;
				return Task.CompletedTask;
			}
			context.Response.Redirect("login");
			return Task.CompletedTask;
		};
	});
	builder.Services.AddTransient<CookieHandler>();
	// HTTP Client
	builder.Services.AddHttpClient("ApiClient", client =>
	{
		client.BaseAddress = new Uri(builder.Configuration["BaseApiUrl"] ?? "https://localhost:7112/");
		client.DefaultRequestHeaders.Accept.Add(
			new MediaTypeWithQualityHeaderValue("application/json"));
	})
	.AddHttpMessageHandler<CookieHandler>();
	// CORS
	builder.Services.AddCors(options =>
	{
		options.AddPolicy("AllowAll", builder =>
		{
			builder.WithOrigins(
					"https://localhost:7112") // Основной домен) 
				.AllowAnyMethod()
				.AllowAnyHeader()
				.AllowCredentials(); // Важно для работы с куками
		});
	});

	// Модули
	foreach (var module in GlobalConfiguration.Modules)
	{
		var moduleInitializerType = module.Assembly.GetTypes()
		   .FirstOrDefault(t => typeof(IModuleInitializer).IsAssignableFrom(t));
		if (moduleInitializerType != null && moduleInitializerType != typeof(IModuleInitializer))
		{
			var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);
			builder.Services.AddSingleton(typeof(IModuleInitializer), moduleInitializer);
			moduleInitializer.ConfigureServices(builder.Services);
		}
	}

	// Swagger
	builder.Services.AddCustomizedSwagger(builder.Configuration);

}

void ConfigureMiddleware()
{
	if (app.Environment.IsDevelopment())
	{
		app.UseExceptionHandler("/Error");
		app.UseStatusCodePagesWithReExecute("/Error/{0}");
		app.UseDeveloperExceptionPage();
		app.UseMigrationsEndPoint();

		// Swagger только для разработки
		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
			c.DisplayRequestDuration();
			c.DefaultModelExpandDepth(2);
			c.DefaultModelsExpandDepth(-1);
			c.DocExpansion(DocExpansion.None);
		});
	}
	else
	{
		app.UseExceptionHandler("/Error");
		app.UseHsts();
	}

	app.UseStaticFiles();
	app.UseRouting();

	app.UseAuthentication();
	app.UseAuthorization();

	app.UseCors("AllowAll");

	app.UseOutputCache();
	app.MapHub<NotificationHub>("/notifications-hub");
	app.UseEndpoints(endpoints =>
	{
		// 1. Маршруты для Account Controller (MVC)
		endpoints.MapControllerRoute(
			name: "account",
			pattern: "Account/{action=Login}",
			defaults: new { controller = "Account" });

		// 2. Маршруты для областей (MVC)
		endpoints.MapControllerRoute(
			name: "areas",
			pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

		// 3. Общий маршрут MVC
		endpoints.MapControllerRoute(
			name: "default",
			pattern: "{controller=Home}/{action=Index}/{id?}");

		// 4. Blazor Hub
		endpoints.MapBlazorHub();

		// 5. Fallback только для Blazor админки
		endpoints.MapFallbackToPage("/admin/{**segment}", "/admin/_Host");
	});

	// Инициализация модулей
	var moduleInitializers = app.Services.GetServices<IModuleInitializer>();
	foreach (var moduleInitializer in moduleInitializers)
	{
		moduleInitializer.Configure(app, builder.Environment);
	}
}
