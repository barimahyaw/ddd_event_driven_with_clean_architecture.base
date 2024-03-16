using DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence;

public class UnitOfWork<T>(T dbContext) : IUnitOfWork
    where T : DbContext
{
    private readonly T _dbContext = dbContext;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _dbContext.SaveChangesAsync(cancellationToken);
}
