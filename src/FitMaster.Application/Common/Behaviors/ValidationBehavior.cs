using FluentValidation;
using MediatR;
using ValidationException = FitMaster.Application.Common.Exceptions.ValidationException;

namespace FitMaster.Application.Common.Behaviors;

/// <summary>
/// Runs before every MediatR request handler. If any FluentValidation validator
/// registered for this request type fails, the handler never runs - a
/// <see cref="ValidationException"/> is thrown instead (caught by the WebApi's
/// global exception handler and turned into a 400 response).
/// </summary>
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}
