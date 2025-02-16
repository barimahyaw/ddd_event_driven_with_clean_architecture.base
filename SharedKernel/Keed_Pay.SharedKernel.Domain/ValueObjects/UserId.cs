using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.ValueObjects;

public class UserId(Ulid id) : ValueObject
{
    public Ulid Id { get; private set; } = id;

    public static Result Validate(Ulid id)
    {
        if (id == Ulid.Empty || id == default)
            return Result.Fail(Errors.UserErrors.UserIdEmpty);

        return Result.Success();
    }

    public static UserId Create(Ulid id)
        => new(id);

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Id;
    }
}
