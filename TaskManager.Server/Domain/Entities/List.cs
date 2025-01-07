using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities;

[TableName("Lists")]
[ColumnIdName("ListId")]
public class List
{
    public Guid ListId { get; set; }
    public string Name { get; set; }
    public int Priority { get; set; }
}
