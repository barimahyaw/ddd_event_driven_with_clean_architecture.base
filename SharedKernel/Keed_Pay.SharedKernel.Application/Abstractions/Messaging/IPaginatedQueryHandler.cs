using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;
using MediatR;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Messaging;

public interface IPaginatedQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, PaginatedResult<TResponse>>
    where TQuery : IPaginatedQuery<TResponse>
{
}