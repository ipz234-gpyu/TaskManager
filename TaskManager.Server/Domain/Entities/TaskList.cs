using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities;

[TableName("TaskLists")]
public class TaskList
{
    public Guid ListId { get; set; }
    public Guid TaskId { get; set; }
}
