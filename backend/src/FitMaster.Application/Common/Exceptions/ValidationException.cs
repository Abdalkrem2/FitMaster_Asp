using FluentValidation.Results;

namespace FitMaster.Application.Common.Exceptions;

/// <summary>
/// Thrown by <c>ValidationBehavior</c> when a Command/Query fails FluentValidation.
/// Maps to HTTP 400 with a field-by-field error breakdown.
/// </summary>
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation failures occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }
}
