namespace TaskManager.Server.Infrastructure.Attributes;

public class ColumnIdNameAttribute : Attribute
{
    public string ColumnIdName { get; init; }

    public ColumnIdNameAttribute(string columnIdName)
    {
        ColumnIdName = columnIdName;
    }
}