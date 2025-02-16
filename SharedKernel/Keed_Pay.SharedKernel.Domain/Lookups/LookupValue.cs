using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Lookups;

public class LookupValue : EntityExtra
{
    public Ulid Id { get; private set; }
    public string ValueName { get; private set; } = null!;
    public string ValueDescription { get; private set; } = null!;
    public long LookupTypeId { get; private set; }
}
