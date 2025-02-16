using DDD_Event_Driven_Clean_Architecture.SharedKernel.Domain.Primitives;

namespace DDD_Event_Driven_Clean_Architecture.SharedKernel.Persistence.Audits;

public sealed class Audit : Entity
{
    public Ulid Id { get; private set; }
    public Ulid UserId { get; private set; }
    public string? Project { get; set; }
    public string? Type { get; private set; }
    public string? TableName { get; private set; }
    public DateTime DateTime { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? AffectedColumns { get; private set; }
    public string? PrimaryKey { get; private set; }

    private Audit() { }

    private Audit(Ulid userId, string project, string type, string tableName, DateTime dateTime, string? oldValues, string? newValues, string? affectedColumns, string primaryKey)
    {
        UserId = userId;
        Project = project;
        Type = type;
        TableName = tableName;
        DateTime = dateTime;
        OldValues = oldValues;
        NewValues = newValues;
        AffectedColumns = affectedColumns;
        PrimaryKey = primaryKey;
    }

    public static Audit Create(Ulid userId, string project, string type, string tableName, DateTime dateTime, string? oldValues, string? newValues, string? affectedColumns, string primaryKey)
        => new(userId, project, type, tableName, dateTime, oldValues, newValues, affectedColumns, primaryKey);
}
