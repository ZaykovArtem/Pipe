using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Infrastructure.Extensions
{
	public static class QueryableExtension
	{
		public static IQueryable<T> OrderByName<T>(this IQueryable<T> source, string propertyName, bool isDescending)
		{
			ArgumentNullException.ThrowIfNull(source);
			if (string.IsNullOrWhiteSpace(propertyName))
			{
				throw new ArgumentException("Property name cannot be empty", nameof(propertyName));
			}
			try
			{
				var parameter = Expression.Parameter(typeof(T), "x");
		
				var property = Expression.Property(parameter, propertyName);
	

			var lambda = Expression.Lambda(property, parameter);

			var methodName = isDescending ? "OrderByDescending" : "OrderBy";
			var method = GetOrderMethod(methodName, typeof(T), property.Type);

			return (IQueryable<T>)method.Invoke(null, new object[] { source, lambda })!;
			}
			catch (Exception ex)
			{
				throw new Exception();
			}
		}

		private static MethodInfo GetOrderMethod(string methodName, Type entityType, Type propertyType)
		{
			return typeof(Queryable).GetMethods()
				.Where(m => m.Name == methodName)
				.Single(m => m.GetParameters().Length == 2)
				.MakeGenericMethod(entityType, propertyType);
		}
	}
}
