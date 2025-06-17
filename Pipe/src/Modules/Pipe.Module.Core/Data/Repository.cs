using System;
using Pipe.Infrastructure.Data;
using Pipe.Infrastructure.Models;

namespace Pipe.Module.Core.Data
{
	public class Repository<T> : RepositoryWithTypedId<T, Guid>, IRepository<T>
	   where T : class, IEntityWithTypedId<Guid>
	{
		public Repository(PipeDbContext context) : base(context)
		{
		}
	}
}
