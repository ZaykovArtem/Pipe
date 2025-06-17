using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Infrastructure.Models
{
	public interface IExtendableObject
	{
		string ExtensionData { get; set; }
	}
}
