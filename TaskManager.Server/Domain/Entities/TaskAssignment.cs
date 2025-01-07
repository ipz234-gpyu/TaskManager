using TaskManager.Server.Infrastructure.Attributes;

namespace TaskManager.Server.Domain.Entities;

[TableName("TaskAssignments")]
public class TaskAssignment
{
    public Guid UserTeamId { get; set; }
    public Guid TaskId { get; set; }
}
