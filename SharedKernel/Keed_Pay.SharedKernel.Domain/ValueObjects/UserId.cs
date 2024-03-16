using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.ValueObjects;

public class UserId(Guid id) : ValueObject
{
    public Guid Id { get; private set; } = id;

    public static Result Validate(Guid id)
    {
        if (id == Guid.Empty)
            return Result.Fail(Errors.UserErrors.UserIdEmpty);

        return Result.Success();
    }

    public static UserId Create(Guid id)
        => new(id);

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Id;
    }
}
