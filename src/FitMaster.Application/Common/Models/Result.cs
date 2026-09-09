namespace FitMaster.Application.Common.Models;

/// <summary>
/// Wraps the outcome of an operation that can fail in an *expected* way
/// (e.g. "package not found", "phone already registered") without throwing
/// an exception for it. Reserve actual exceptions for truly exceptional,
/// unexpected failures - see Common/Exceptions.
/// </summary>
public class Result
{
    public bool Succeeded { get; }

    public string[] Errors { get; }

    protected Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public static Result Success() => new(true, []);

    public static Result Failure(params string[] errors) => new(false, errors);
}

/// <summary>Same as <see cref="Result"/>, but carries a value on success.</summary>
public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(bool succeeded, T? value, IEnumerable<string> errors)
        : base(succeeded, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, []);

    public new static Result<T> Failure(params string[] errors) => new(false, default, errors);
}
