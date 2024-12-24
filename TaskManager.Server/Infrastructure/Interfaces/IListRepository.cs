using TaskManager.Server.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Server.Infrastructure.Interfaces
{
    public interface IListRepository : IRepository<List>
    {
        Task<List> CreateForUserAsync(Guid groupId, List entity);
        Task<List> CreateForTeamAsync(Guid groupId, List entity);

        Task<IEnumerable<List>> GetAllForGroupUserAsync(Guid groupId);
        Task<IEnumerable<List>> GetAllForGroupTeamAsync(Guid groupId);

        Task CreateConnectionForUser(Guid groupId, Guid listId);
        Task CreateConnectionForTeam(Guid groupId, Guid listId);
        Task DeleteConnectionForUser(Guid groupId, Guid listId);
        Task DeleteConnectionForTeam(Guid groupId, Guid listId);
    }
}
