using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;
using System.ComponentModel.DataAnnotations;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Lookups;

public class LookupType : EntityExtra
{
    public long Id { get; private set; }
    [Required, MaxLength(150)]
    public string TypeName { get; private set; } = null!;
    [Required, MaxLength(255)]
    public string Description { get; private set; } = null!;

    private readonly HashSet<LookupValue> _lookupValues = [];
    public IReadOnlyList<LookupValue> LookupValues => _lookupValues.ToList();
}
