using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities;

[TableName("Tasks")]
[ColumnIdName("TaskId")]
public class Task
{
    public Guid TaskId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? Notification { get; set; }
    public int Priority { get; set; }
    public string Status { get; set; }
    public Guid? ParentTaskId { get; set; }
}
