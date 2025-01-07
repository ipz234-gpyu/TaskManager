using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities;

[TableName("Tags")]
[ColumnIdName("TagId")]
public class Tag
{
    public Guid TagId { get; set; }
    public string Name { get; set; }
    public string? Color { get; set; }
    public Guid? UserId { get; set; }
}
