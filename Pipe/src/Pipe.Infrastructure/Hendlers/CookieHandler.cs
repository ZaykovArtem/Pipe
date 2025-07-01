using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Infrastructure.Hendlers
{

	public class CookieHandler : DelegatingHandler
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CookieHandler(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		protected override async Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			// Добавляем куки из текущего контекста
			var cookies = _httpContextAccessor.HttpContext?.Request.Cookies;
			if (cookies != null && cookies.Count > 0)
			{
				request.Headers.Add("Cookie", string.Join("; ", cookies.Select(c => $"{c.Key}={c.Value}")));
			}

			return await base.SendAsync(request, cancellationToken);
		}
	}
}
