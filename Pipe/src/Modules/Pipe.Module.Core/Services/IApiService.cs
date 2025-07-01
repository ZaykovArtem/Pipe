using Microsoft.Extensions.Configuration;
using Pipe.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Services
{
	public interface IApiService
	{
		public Task<Result<T>> GetAsync<T>(string endpoint);
		public Task<Result<T>> PostAsync<T>(string endpoint, object data);
		public Task<Result<T>> PutAsync<T>(string endpoint, object data);
		public Task<Result> DeleteAsync(string endpoint);
		Task<Result<T>> PatchAsync<T>(string endpoint, object data);
	}
}
