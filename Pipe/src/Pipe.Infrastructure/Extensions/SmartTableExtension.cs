using Microsoft.EntityFrameworkCore;
using Pipe.Infrastructure.SmartTable;
using System.Linq.Expressions;

namespace Pipe.Infrastructure.Extensions
{
	public static class SmartTableExtension
	{
		public static async Task<SmartTableResult<TModel>> ToSmartTableResultAsync<TModel>(this IQueryable<TModel> query, SmartTableParam param)
		{
			var totalRecord = await query.CountAsync();
			var items = await query.AppendSortAndPagingation(param).ToListAsync();

			return new SmartTableResult<TModel>
			{
				Items = items,
				TotalRecord = totalRecord,
				NumberOfPages = (int)Math.Ceiling((double)totalRecord / param.Pagination.Number)
			};
		}

		public static async Task<SmartTableResult<TResult>> ToSmartTableResultAsync<TModel, TResult>(this IQueryable<TModel> query, SmartTableParam param, Expression<Func<TModel, TResult>> selector)
		{
			var totalRecord = await query.CountAsync();
			query = query.AppendSortAndPagingation(param);
			var items = await query.Select(selector).ToListAsync();

			return new SmartTableResult<TResult>
			{
				Items = items,
				TotalRecord = totalRecord,
				NumberOfPages = (int)Math.Ceiling((double)totalRecord / param.Pagination.Number)
			};
		}

		// ToLits() before Select() to loaded related many-to-many entity
		public static async Task<SmartTableResult<TResult>> ToSmartTableResultNoProjectionAsync<TModel, TResult>(this IQueryable<TModel> query, SmartTableParam param, Expression<Func<TModel, TResult>> selector)
		{
			var totalRecord = await query.CountAsync();
			var items = await query.AppendSortAndPagingation(param).ToListAsync();

			return new SmartTableResult<TResult>
			{
				Items = items.AsQueryable().Select(selector),
				TotalRecord = totalRecord,
				NumberOfPages = (int)Math.Ceiling((double)totalRecord / param.Pagination.Number)
			};
		}

		private static IQueryable<TModel> AppendSortAndPagingation<TModel>(this IQueryable<TModel> query, SmartTableParam param)
		{
			if (param.Pagination.Number <= 0)
				param.Pagination.Number = 10;

			if (!string.IsNullOrWhiteSpace(param.Sort.Predicate))
				query = query.OrderByName(param.Sort.Predicate, param.Sort.Reverse);
			else
				query = query.OrderByName("Id", true);

			query = query
				.Skip(param.Pagination.Start)
				.Take(param.Pagination.Number);

			return query;
		}
	}
}
