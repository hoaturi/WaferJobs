using FluentValidation;
using JobBoard.Common.Exceptions;
using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Common.Behaviors;

public class RequestValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> requestValidators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!requestValidators.Any()) return await next();

        var validationContext = new ValidationContext<TRequest>(request);

        var validationResults =
            await Task.WhenAll(requestValidators.Select(validator =>
                validator.ValidateAsync(validationContext, cancellationToken)));

        var validationFailures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .GroupBy(failure => failure.PropertyName)
            .Select(group =>
                new ValidationError(group.Key, string.Join(", ", group.Select(failure => failure.ErrorMessage))))
            .ToList();


        if (validationFailures.Count > 0) throw new CustomValidationException(validationFailures);

        return await next();
    }
}