using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.ValueObjects;

public class UserId : ValueObject
{
    public long Id { get; private set; }

    private UserId(long id) =>
        Id = id;

    public static Result Validate(long id)
    {
        if (id == 0)
            return Result.Fail(Errors.UserErrors.UserIdEmpty);

        return Result.Success();
    }

    public static UserId Create(long id)
        => new(id);

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Id;
    }
}
