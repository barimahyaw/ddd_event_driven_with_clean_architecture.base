using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Audit;

public sealed class Audit : Entity
{
    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string? Type { get; private set; }
    public string? TableName { get; private set; }
    public DateTime DateTime { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? AffectedColumns { get; private set; }
    public string? PrimaryKey { get; private set; }

    private Audit() { }

    public Audit(long userId, string type, string tableName, DateTime dateTime, string? oldValues, string? newValues, string? affectedColumns, string primaryKey)
    {
        UserId = userId;
        Type = type;
        TableName = tableName;
        DateTime = dateTime;
        OldValues = oldValues;
        NewValues = newValues;
        AffectedColumns = affectedColumns;
        PrimaryKey = primaryKey;
    }

    public static Audit Create(long userId, string type, string tableName, DateTime dateTime, string? oldValues, string? newValues, string? affectedColumns, string primaryKey)
        => new(userId, type, tableName, dateTime, oldValues, newValues, affectedColumns, primaryKey);
}
