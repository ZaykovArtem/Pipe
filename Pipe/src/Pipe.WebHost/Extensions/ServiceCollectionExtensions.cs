using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
			// Регистрация Identity с кастомными хранилищами
			services.AddIdentity<User, Role>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequiredLength = 4;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
				options.Password.RequiredUniqueChars = 0;
				options.ClaimsIdentity.UserNameClaimType = JwtRegisteredClaimNames.Sub;

				// Дополнительные настройки
				options.User.RequireUniqueEmail = true;
				options.SignIn.RequireConfirmedAccount = false;
			})
			.AddRoleStore<PipeRoleStore>()
			.AddUserStore<PipeUserStore>()
			.AddSignInManager<PipeSignInManager<User>>()
			.AddDefaultTokenProviders();

			// Настройка аутентификации
			services.AddAuthentication(options =>
			{
				options.DefaultScheme = IdentityConstants.ApplicationScheme;
				options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
			})
			.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
			{
				options.LoginPath = "/login";
				options.LogoutPath = "/logout";
				options.AccessDeniedPath = "/access-denied";
				options.ExpireTimeSpan = TimeSpan.FromDays(30);
				options.SlidingExpiration = true;
				options.Cookie.HttpOnly = true;
				options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
			});

			// Обязательные сервисы для Identity
			services.AddScoped<IUserClaimsPrincipalFactory<User>, UserClaimsPrincipalFactory<User, Role>>();
			services.AddScoped<ISecurityStampValidator, SecurityStampValidator<User>>();
			//services.AddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<User>>();
			services.AddScoped<IUserConfirmation<User>, DefaultUserConfirmation<User>>();

			// Настройка кук приложения
			services.ConfigureApplicationCookie(options =>
			{
				options.Cookie.SameSite = SameSiteMode.None;
				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

				options.Events.OnRedirectToLogin = context =>
				{
					if (context.Request.Path.StartsWithSegments("/api") ||
						context.Request.Path.StartsWithSegments("/swagger"))
					{
						context.Response.StatusCode = StatusCodes.Status401Unauthorized;
						return Task.CompletedTask;
					}
					context.Response.Redirect(context.RedirectUri);
					return Task.CompletedTask;
				};

				options.Events.OnRedirectToAccessDenied = context =>
				{
					if (context.Request.Path.StartsWithSegments("/api") ||
						context.Request.Path.StartsWithSegments("/swagger"))
					{
						context.Response.StatusCode = StatusCodes.Status403Forbidden;
						return Task.CompletedTask;
					}
					context.Response.Redirect("login");
					return Task.CompletedTask;
				};
			});

			return services;
		}
		public static IServiceCollection AddCustomizedSwagger(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Documentation", Version = "v1" });
				// Настраиваем только API из определенного сегмента
				c.DocInclusionPredicate((docName, apiDesc) =>
				{
					// Фильтруем только контроллеры с маршрутом, содержащим "/api/"
					return apiDesc.RelativePath?.StartsWith("api/", StringComparison.OrdinalIgnoreCase) == true;
				});
				foreach (var module in GlobalConfiguration.Modules)
				{

					var xmlFile = $"{module.Id}.xml";
					var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
					if (File.Exists(xmlPath))
					{
						// Указываем сборку с API-контроллерами
						c.IncludeXmlComments(xmlPath);
					}
					else
					{
						// Логирование отсутствующего файла (опционально)
						Console.WriteLine($"XML documentation not found: {xmlPath}");
					}
				}
			});
			return services;
		}
	}
}
