using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Infrastructure.SmartTable
{
	public sealed class SmartTableParam
	{
		public static SmartTableParam Create(int number = 10, int numberOfPages = 1)
		{
			var smartTableParam = new SmartTableParam()
			{
				Pagination = new Pagination { Number = number, NumberOfPages = numberOfPages },
				Search = new Search
				{
					PredicateObject = new Dictionary<string, object>()
				},
				Sort = new Sort() { Predicate = string.Empty }
			};
			return smartTableParam;
		}
		public Pagination Pagination { get; set; }

		public Search Search { get; set; }

		public Sort Sort { get; set; }
	}
}
