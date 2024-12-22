using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;
using Task = TaskManager.Server.Domain.Entities.Task;

namespace TaskManager.Server.Infrastructure.Repositories
{
    public class TaskRepository : Repository<Task>, ITaskRepository
    {
        public TaskRepository(ISqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }


    }
}
