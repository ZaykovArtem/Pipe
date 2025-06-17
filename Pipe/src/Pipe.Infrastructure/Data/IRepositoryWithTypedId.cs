using Microsoft.EntityFrameworkCore.Storage;
using Pipe.Infrastructure.Models;

namespace Pipe.Infrastructure.Data
{
	public interface IRepositoryWithTypedId<T, TId> where T : IEntityWithTypedId<TId>
	{
		IQueryable<T> Query();
		void Add(T entity);
		void AddRange(IEnumerable<T> entities);
		void Remove(T entity);
		void SaveChanges();
		IDbContextTransaction BeginTransaction();

		// Асинхронные методы
		Task AddAsync(T entity);
		Task AddRangeAsync(IEnumerable<T> entities);
		Task SaveChangesAsync();
		Task<IDbContextTransaction> BeginTransactionAsync();
		Task BulkInsertAsync(IEnumerable<T> entities);
	}
}
