using GraphQL.Types;
using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.API.Teams.Types;

public class TaskAssignmentResponsType : ObjectGraphType<TaskAssignment>
{
    public TaskAssignmentResponsType()
    {
        Field(t => t.UserTeamId);
        Field(t => t.TaskId);
    }
}
