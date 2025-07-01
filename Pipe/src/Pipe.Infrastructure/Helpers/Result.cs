using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pipe.Infrastructure.Helpers
{
	/// <summary>
	/// Универсальный результат операции с поддержкой статусов Success/Failure/NotFound
	/// </summary>
	public class Result
	{
		public bool IsSuccess { get; }
		public bool IsFailure => !IsSuccess;
		public bool IsNotFound { get; }
		public string Message { get; }

		protected Result(bool isSuccess, bool isNotFound, string message)
		{
			IsSuccess = isSuccess;
			IsNotFound = isNotFound;
			Message = message;
		}

		public static Result Success() => new Result(true, false, null);
		public static Result Success(string message) => new Result(true, false, message);
		public static Result Failure(string error) => new Result(false, false, error);
		public static Result NotFound(string error) => new Result(false, true, error);
	}

	/// <summary>
	/// Универсальный результат операции с возвращаемым значением
	/// </summary>
	public class Result<T> : Result
	{
		public T Value { get; }

		protected Result(bool isSuccess, bool isNotFound, string message, T value)
			: base(isSuccess, isNotFound, message)
		{
			Value = value;
		}

		public static Result<T> Success(T value) => new Result<T>(true, false, null, value);
		public static Result<T> Success(T value, string message) => new Result<T>(true, false, message, value);
		public static new Result<T> Failure(string error) => new Result<T>(false, false, error, default);
		public static new Result<T> NotFound(string error) => new Result<T>(false, true, error, default);
	}
}
