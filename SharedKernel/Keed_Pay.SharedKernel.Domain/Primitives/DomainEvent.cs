namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;

public record DomainEvent(long Id) : IDomainEvent;
