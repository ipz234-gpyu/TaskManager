using TaskModel = TaskManager.Server.Domain.Entities.Task;

namespace TaskManager.Server.Infrastructure.Interfaces;

public interface ITaskRepository : IRepository<TaskModel>
{
    Task<TaskModel> CreateForList(TaskModel entity, Guid listId);
    Task<IEnumerable<TaskModel>> GetAllForList(Guid listId);
    Task CreateConnection(Guid listId, Guid taskId);
    Task DeleteConnection(Guid listId, Guid taskId);
    Task<IEnumerable<TaskModel>> GetUserTasksInDateRange(Guid userId, DateTime startTime, DateTime endTime);
    Task<IEnumerable<TaskModel>> GetTeamTasksInDateRange(Guid teamId, DateTime startTime, DateTime endTime);
    Task AssignTaskToTeamMember(Guid userId, Guid teamId, Guid taskId);
    Task RemoveTaskFromTeamMember(Guid userId, Guid teamId, Guid taskId);
    Task<IEnumerable<TaskModel>> GetUserTasksByTags(Guid userId, IEnumerable<Guid> tagIds);
    Task<IEnumerable<TaskModel>> GetTeamTasksByTags(Guid teamId, IEnumerable<Guid> tagIds);
}
