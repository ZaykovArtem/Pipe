using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Infrastructure.SmartTable
{
	public class SmartTableResult<T>
	{
		public required IEnumerable<T> Items { get; set; }

		public int TotalRecord { get; set; }

		public int NumberOfPages { get; set; }
	}
}
