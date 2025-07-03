using Microsoft.EntityFrameworkCore;
using Pipe.Infrastructure.Data;
using Pipe.Module.Notifications.Models;

namespace Pipe.Module.Notifications.Data
{
	internal class NotificationsCustomModelBuilder : ICustomModelBuilder
	{
		public void Build(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Notification>()
			.HasIndex(n => n.CreatedAt);
		}
	}
}
