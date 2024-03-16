﻿using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;
using System.Text.RegularExpressions;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.ValueObjects;

public sealed partial class EmailAddress : ValueObject
{
    public string Value { get; private set; } = default!;

    private EmailAddress(string value) =>
        Value = value;

    internal const int MaxLength = 30;

    internal const int MinLength = 6;

    public static Result Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail(Errors.EmailAddressErrors.EmailAddressNullOrEmpty);

        var emailRegex = EmailValidityExpression();
        if (!emailRegex.IsMatch(value))
            return Result.Fail(Errors.EmailAddressErrors.EmailAddressInvalid);

        if (value.Length < MinLength || value.Length > MaxLength)
            return Result.Fail(Errors.EmailAddressErrors.EmailAddressTooShortOrTooLong);

        return Result.Success();
    }

    public static EmailAddress Create(string value)
        => new(value);

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")]
    private static partial Regex EmailValidityExpression();
}
