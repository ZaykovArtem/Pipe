using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Pipe.Infrastructure.Modules;


namespace Pipe.Module.Core
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
			serviceCollection.AddMudServices();
		}

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
