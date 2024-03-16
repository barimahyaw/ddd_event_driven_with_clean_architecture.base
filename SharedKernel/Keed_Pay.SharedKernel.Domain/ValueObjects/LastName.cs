﻿using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.ValueObjects;

public sealed class LastName : ValueObject
{
    public string Value { get; private set; } = default!;

    private LastName(string value) =>
        Value = value;

    internal const int MaxLength = 50;

    public static Result Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail(Errors.LastNameErrors.LastNameNullOrEmpty);

        if (value.Length > 50)
            return Result.Fail(Errors.LastNameErrors.LastNameTooLong);

        return Result.Success();
    }

    public static LastName Create(string value)
        => new(value);

    public static implicit operator string(LastName lastName) => lastName.Value;

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}