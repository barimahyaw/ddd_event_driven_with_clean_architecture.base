using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;
using MediatR;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Application.Abstractions.Messaging;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, IResult>
    where TCommand : ICommand
{
}

public interface ICommandHandler<TCommand, TResponse>
    : IRequestHandler<TCommand, IResult<TResponse>>
    where TCommand : ICommand<TResponse>
{
}
