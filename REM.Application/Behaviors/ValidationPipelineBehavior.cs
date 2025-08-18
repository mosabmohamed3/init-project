using FluentValidation;
using FluentValidation.Results;

namespace REM.Application.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators) =>
        _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

        var errors = validationResults
            .Where(result => !result.IsValid)
            .SelectMany(result => result.Errors)
            .Select(ValidationException => new ValidationFailure(
                ValidationException.PropertyName,
                ValidationException.ErrorMessage
            ))
            .ToList();

        if (!errors.Any())
            return await next();
        
        throw new Exceptions.ValidationException(errors);
    }
}
