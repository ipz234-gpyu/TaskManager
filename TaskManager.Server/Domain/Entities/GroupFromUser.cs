using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities;

[TableName("GroupsFromUser")]
[ColumnIdName("GroupsFromUserId")]
public class GroupFromUser
{
    public Guid GroupsFromUserId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public int Priority { get; set; }
}
