using Task = TaskManager.Server.Domain.Entities.Task;

namespace TaskManager.Server.Infrastructure.Interfaces
{
    public interface ITaskRepository : IRepository<Task>
    {
    }
}
