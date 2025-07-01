using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pipe.Infrastructure.SmartTable
{
	public class Search
	{
		public Dictionary<string, object> PredicateObject { get; set; } = new();
	}


}
