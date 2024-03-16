using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.ValueObjects;

public class OtherName : ValueObject
{
    public string Value { get; private set; } = default!;

    private OtherName(string value) =>
        Value = value;

    internal const int MaxLength = 50;

    public static Result Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Success();

        if (!string.IsNullOrWhiteSpace(value) && value.Length > 50)
            return Result.Fail(Errors.OtherNameErrors.OtherNameTooLong);

        return Result.Success();
    }

    public static OtherName Create(string value)
        => new(value);

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}