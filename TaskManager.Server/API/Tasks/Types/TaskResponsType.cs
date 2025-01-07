using GraphQL.Types;
using TaskManager.Server.Infrastructure.Interfaces;
using Task = TaskManager.Server.Domain.Entities.Task;

namespace TaskManager.Server.API.Tasks.Types;

public class TaskResponsType : ObjectGraphType<Task>
{
    public TaskResponsType(ITagRepository tagRepository)
    {
        Field(t => t.TaskId);
        Field(t => t.Title);
        Field(t => t.Description);
        Field(t => t.StartTime);
        Field(t => t.Deadline);
        Field(t => t.Notification);
        Field(t => t.Priority);
        Field(t => t.Status);
        Field(t => t.ParentTaskId);

        FieldAsync<ListGraphType<GuidGraphType>>(
       "tags",
        resolve: async context =>
        {
           var taskId = context.Source.TaskId;
           return await tagRepository.GetTagIdsByTaskId(taskId);
        });
    }
}
