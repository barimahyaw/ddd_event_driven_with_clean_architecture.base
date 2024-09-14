using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;
using MediatR;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Messaging;

public interface IPaginatedQuery<TResponse> : IRequest<PaginatedResult<TResponse>>
{
}
