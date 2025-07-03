using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Pipe.Infrastructure.Modules;
using Pipe.Module.Core.Events;
using Pipe.Module.Notifications.Hubs;
using Pipe.Module.Notifications.Services;


namespace Pipe.ModuleюТ.Notifications
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void ConfigureServices(IServiceCollection serviceCollection)
        {
			serviceCollection.AddScoped<INotificationHandler<UserCreatedEvent>, NotificationHandler>();
			serviceCollection.AddScoped<INotificationHandler<UserUpdatedEvent>, NotificationHandler>();
			serviceCollection.AddScoped<INotificationHandler<UserDeletedEvent>, NotificationHandler>();
			serviceCollection.AddScoped<INotificationService, NotificationService>();
		}

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
