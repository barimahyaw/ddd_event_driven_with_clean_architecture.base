namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Data;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
