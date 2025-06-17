using Pipe.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Infrastructure.Data
{
	public interface IRepository<T> : IRepositoryWithTypedId<T, Guid> where T : IEntityWithTypedId<Guid>
	{
	}
}
