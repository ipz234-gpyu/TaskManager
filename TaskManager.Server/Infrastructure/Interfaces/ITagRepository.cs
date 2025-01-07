using TaskManager.Server.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Server.Infrastructure.Interfaces;

public interface ITagRepository : IRepository<Tag>
{
    Task<IEnumerable<Tag>> GetAllForUser(Guid userId);
    Task<IEnumerable<Tag>> GetAllForTeam(Guid teamId);
    Task CreateConnection(Guid tagId, Guid taskId);
    Task DeleteConnection(Guid tagId, Guid taskId);
    Task<IEnumerable<Guid>> GetTagIdsByTaskId(Guid taskId);
}
