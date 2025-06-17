using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Pipe.Infrastructure.Data;
using Pipe.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCore.BulkExtensions;

namespace Pipe.Module.Core.Data
{
	public class RepositoryWithTypedId<T, TId> : IRepositoryWithTypedId<T, TId> where T : class, IEntityWithTypedId<TId>
	{
		public RepositoryWithTypedId(PipeDbContext context)
		{
			Context = context;
			DbSet = Context.Set<T>();
		}

		protected DbContext Context { get; }

		protected DbSet<T> DbSet { get; }

		public IQueryable<T> Query()
		{
			return DbSet;
		}


		public void Add(T entity)
		{
			DbSet.Add(entity);
		}

		public void AddRange(IEnumerable<T> entity)
		{
			DbSet.AddRange(entity);
		}

		public IDbContextTransaction BeginTransaction()
		{
			return Context.Database.BeginTransaction();
		}

		public void SaveChanges()
		{
			Context.SaveChanges();
		}

		public async Task SaveChangesAsync()
		{
			await Context.SaveChangesAsync();
		}


		public void Remove(T entity)
		{
			DbSet.Remove(entity);
		}

		public async Task AddAsync(T entity)
		{
			await DbSet.AddAsync(entity);
		}

		public async Task AddRangeAsync(IEnumerable<T> entities)
		{
			await DbSet.AddRangeAsync(entities);	
		}

		public async Task<IDbContextTransaction> BeginTransactionAsync()
		{
			return await Context.Database.BeginTransactionAsync();
		}

		public async Task BulkInsertAsync(IEnumerable<T> entities)
		{
			await Context.BulkInsertAsync(entities);
		}
	}
}
