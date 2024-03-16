using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;
using MediatR;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, IResult<TResponse>>
    where TQuery : IQuery<TResponse>
{
}