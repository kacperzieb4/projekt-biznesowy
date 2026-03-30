using System;

namespace AttractionCatalog.Application.Common.Models
{
    public record Result
    {
        public bool IsSuccess { get; init; }
        public string Error { get; init; }
        public static Result Success() => new() { IsSuccess = true, Error = "" };
        public static Result Failure(string error) => new() { IsSuccess = false, Error = error };
    }

    public record Result<T> : Result
    {
        public T Value { get; init; }
        public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value, Error = "" };
        public new static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
    }
}
