using Microsoft.EntityFrameworkCore;
namespace Pipe.Infrastructure.Data
{
	public interface ICustomModelBuilder
	{
		void Build(ModelBuilder modelBuilder);
	}
}
