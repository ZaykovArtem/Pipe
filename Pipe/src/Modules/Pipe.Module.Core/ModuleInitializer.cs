using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using Pipe.Infrastructure.Modules;
using Pipe.Module.Core.Services;


namespace Pipe.Module.Core
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
			serviceCollection.AddScoped<ITokenProvider, TokenProvider>();
			serviceCollection.AddScoped<IUserService, UserService>();
			serviceCollection.AddScoped<IRoleService, RoleService>();
			serviceCollection.AddScoped<IApiService, ApiService>();
			serviceCollection.AddMudServices();
			serviceCollection.AddMudServices(config => {
				config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
				config.SnackbarConfiguration.PreventDuplicates = true;
				config.SnackbarConfiguration.MaxDisplayedSnackbars = 3;
			});
		}

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
