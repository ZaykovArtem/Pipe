using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pipe.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pipe.Module.Core.Services
{
	public class ApiService : IApiService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IConfiguration _configuration;
		private readonly NavigationManager _navigation;
		private readonly ITokenProvider _tokenProvider;

		public ApiService(
			IHttpClientFactory httpClientFactory,
			IConfiguration configuration,
			NavigationManager navigation,
			ITokenProvider tokenProvider)
		{
			_httpClientFactory = httpClientFactory;
			_configuration = configuration;
			_navigation = navigation;
			_tokenProvider = tokenProvider;
		}

		public async Task<Result<T>> GetAsync<T>(string endpoint)
		{
			try
			{
				using var client = await CreateHttpClient();
				var response = await client.GetAsync(endpoint);

				return await HandleResponse<T>(response);
			}
			catch (Exception ex)
			{
				return Result<T>.Failure($"API request failed: {ex.Message}");
			}
		}

		public async Task<Result<T>> PostAsync<T>(string endpoint, object data)
		{
			try
			{
				using var client = await CreateHttpClient();
				var response = await client.PostAsJsonAsync(endpoint, data);

				return await HandleResponse<T>(response);
			}
			catch (Exception ex)
			{
				return Result<T>.Failure($"API request failed: {ex.Message}");
			}
		}

		public async Task<Result<T>> PutAsync<T>(string endpoint, object data)
		{
			try
			{
				using var client = await CreateHttpClient();
				var response = await client.PutAsJsonAsync(endpoint, data);

				return await HandleResponse<T>(response);
			}
			catch (Exception ex)
			{
				return Result<T>.Failure($"API request failed: {ex.Message}");
			}
		}

		public async Task<Result> DeleteAsync(string endpoint)
		{
			try
			{
				using var client = await CreateHttpClient();
				var response = await client.DeleteAsync(endpoint);

				return await HandleResponse(response);
			}
			catch (Exception ex)
			{
				return Result.Failure($"API request failed: {ex.Message}");
			}
		}

		public async Task<Result<T>> PatchAsync<T>(string endpoint, object data)
		{
			try
			{
				using var client = await CreateHttpClient();
				var response = await client.PatchAsJsonAsync(endpoint, data);

				return await HandleResponse<T>(response);
			}
			catch (Exception ex)
			{
				return Result<T>.Failure($"API request failed: {ex.Message}");
			}
		}

		private async Task<HttpClient> CreateHttpClient()
		{
			var client = _httpClientFactory.CreateClient("ApiClient");

			// Добавляем токен авторизации
			var token = await _tokenProvider.GetAccessTokenAsync();
			if (!string.IsNullOrEmpty(token))
			{
				client.DefaultRequestHeaders.Authorization =
					new AuthenticationHeaderValue("Bearer", token);
			}

			return client;
		}

		private async Task<Result<T>> HandleResponse<T>(HttpResponseMessage response)
		{
			switch (response.StatusCode)
			{
				case HttpStatusCode.Unauthorized:
					_navigation.NavigateTo("/logout");
					return Result<T>.Failure("Authentication required");

				case HttpStatusCode.Forbidden:
					return Result<T>.Failure("Access denied");

				case HttpStatusCode.NotFound:
					return Result<T>.NotFound("Resource not found");

				case HttpStatusCode.NoContent:
					return Result<T>.Success(default, "Operation successful");

				default:
					if (response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsStringAsync();

						if (string.IsNullOrEmpty(content))
							return Result<T>.Success(default);

						try
						{
							var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
							{
								PropertyNameCaseInsensitive = true
							});
							return Result<T>.Success(result);
						}
						catch (JsonException ex)
						{
							return Result<T>.Failure($"Failed to parse response: {ex.Message}");
						}
					}

					var errorContent = await response.Content.ReadAsStringAsync();
					return Result<T>.Failure($"API error: {response.StatusCode} - {errorContent}");
			}
		}

		private async Task<Result> HandleResponse(HttpResponseMessage response)
		{
			switch (response.StatusCode)
			{
				case HttpStatusCode.Unauthorized:
					_navigation.NavigateTo("/logout");
					return Result.Failure("Authentication required");

				case HttpStatusCode.Forbidden:
					return Result.Failure("Access denied");

				case HttpStatusCode.NotFound:
					return Result.NotFound("Resource not found");

				case HttpStatusCode.NoContent:
				case HttpStatusCode.OK:
					return Result.Success("Operation successful");

				default:
					if (response.IsSuccessStatusCode)
					{
						return Result.Success();
					}

					var errorContent = await response.Content.ReadAsStringAsync();
					return Result.Failure($"API error: {response.StatusCode} - {errorContent}");
			}
		}
	}

	// Интерфейс для провайдера токенов
	public interface ITokenProvider
	{
		Task<string> GetAccessTokenAsync();
	}

	// Реализация для Blazor WebAssembly
	public class TokenProvider : ITokenProvider
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public TokenProvider(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public Task<string> GetAccessTokenAsync()
		{
			return Task.FromResult(_httpContextAccessor.HttpContext?.Request.Cookies["access_token"]);
		}
	}
}
