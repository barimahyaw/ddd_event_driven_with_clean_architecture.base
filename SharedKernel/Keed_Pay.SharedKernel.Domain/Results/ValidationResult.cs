namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;

public record ValidationResult : Result, IValidationResult
{
    private ValidationResult(Error[] errors)
        : base(false, IValidationResult.ValidationError) =>
        Messages = errors;

    public static ValidationResult WithErrors(Error[] errors) =>
        new(errors);
}
