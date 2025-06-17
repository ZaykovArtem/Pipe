using Pipe.Infrastructure.Models;
using System.ComponentModel.DataAnnotations;


namespace Pipe.Module.Core.Models
{
	public class AppSetting : EntityBaseWithTypedId<string>
	{
		public AppSetting(string id)
		{
			Id = id;
		}

		[StringLength(450)]
		public string? Value { get; set; }

		[StringLength(450)]
		public string? Module { get; set; }
	}
}
