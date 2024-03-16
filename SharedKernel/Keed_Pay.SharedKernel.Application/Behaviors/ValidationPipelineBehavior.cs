using FluentValidation;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;
using MediatR;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Behaviors;

public sealed class ValidationPipelineBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            Error[] errors = _validators
                .Select(validator => validator.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(failure => failure != null)
                .Select(failure => new Error(
                    string.IsNullOrWhiteSpace(failure.PropertyName)
                        ? "validation_error"
                        : failure.PropertyName,
                    failure.ErrorMessage))
                .Distinct()
                .ToArray();

            if (errors.Length != 0)
            {
                return CreateValidationResult<TResponse>(errors);
            };
        }

        return await next();
    }

    private static TResult CreateValidationResult<TResult>(Error[] errors)
        where TResult : IResult
    {
        if (typeof(TResult) == typeof(Result))
            return (TResult)(object)ValidationResult.WithErrors(errors);

        if (typeof(TResult) == typeof(ResultT<>))
        {
            return (TResult)(object)ResultT<TResult>.Fail(errors);

        }
        return Equals(typeof(TResult), typeof(Result))
            ? (TResult)(object)ValidationResult.WithErrors(errors)
            : (TResult)(object)ResultT<TResult>.Fail(errors);
    }
}
