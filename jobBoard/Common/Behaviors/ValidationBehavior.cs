using FluentValidation;
using MediatR;

namespace JobBoard;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validator)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validator;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResult = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context))
        );

        var failures = validationResult
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .GroupBy(failure => failure.PropertyName)
            .Select(
                group =>
                    new ValidationError(
                        group.Key,
                        group.Select(failure => failure.ErrorMessage).ToList()
                    )
            )
            .ToList();

        if (failures.Count is not 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
