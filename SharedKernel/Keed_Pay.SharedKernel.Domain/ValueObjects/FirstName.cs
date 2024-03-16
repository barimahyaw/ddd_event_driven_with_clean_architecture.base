using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.ValueObjects;

public sealed class FirstName : ValueObject
{
    public string Value { get; private set; } = default!;

    private FirstName(string value) =>
        Value = value;

    internal const int MaxLength = 50;

    public static Result Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail(Errors.FirstNameErrors.FirstNameNullOrEmpty);

        if (value.Length > 50)
            return Result.Fail(Errors.FirstNameErrors.FirstNameTooLong);

        return Result.Success();
    }

    public static FirstName Create(string value)
        => new(value);

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}