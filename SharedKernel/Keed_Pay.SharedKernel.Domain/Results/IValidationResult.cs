namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;

public interface IValidationResult
{
    public static readonly Error ValidationError = new(
        "Validation error",
        "A validation problem occurred.");

    Error[] Messages { get; }
}