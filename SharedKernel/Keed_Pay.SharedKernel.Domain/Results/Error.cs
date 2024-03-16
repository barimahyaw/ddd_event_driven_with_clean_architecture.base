namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Results;

public record Error(string Code, string Message)
{
    public string Code { get; } = Code;
    public string Message { get; } = Message;
}
