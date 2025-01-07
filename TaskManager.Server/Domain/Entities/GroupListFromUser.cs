using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities;

[TableName("GroupListsFromUser")]
public class GroupListFromUser
{
    public Guid ListId { get; set; }
    public Guid GroupsFromUserId { get; set; }
}
