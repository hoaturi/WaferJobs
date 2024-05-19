using FluentValidation;
using JobBoard.Common.Models;
using MediatR;
using ValidationException = JobBoard.Common.Exceptions.ValidationException;

namespace JobBoard.Common.Behaviors;

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _requestValidators;

    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> requestValidators)
    {
        _requestValidators = requestValidators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_requestValidators.Any()) return await next();

        var validationContext = new ValidationContext<TRequest>(request);

        var validationResults =
            await Task.WhenAll(_requestValidators.Select(validator => validator.ValidateAsync(validationContext)));

        var validationFailures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .GroupBy(failure => failure.PropertyName)
            .Select(group =>
                new ValidationError(group.Key, string.Join(", ", group.Select(failure => failure.ErrorMessage))))
            .ToList();

        if (validationFailures.Count > 0) throw new ValidationException(validationFailures);

        return await next();
    }
}