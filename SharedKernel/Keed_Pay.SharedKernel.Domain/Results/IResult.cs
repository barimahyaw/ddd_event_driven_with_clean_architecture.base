namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;

public interface IResult
{
    bool Succeeded { get; }
    Error[] Messages { get; }
}

public interface IResult<out T> : IResult
{
    T Data { get; }
}