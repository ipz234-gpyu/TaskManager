using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.Infrastructure.Interfaces
{
    public interface IListRepository : IRepository<List>
    {
        Task<List> CreateForUserAsync(Guid groupId, List entity);
        Task<List> CreateForTeamAsync(Guid groupId, List entity);

        Task<IEnumerable<List>> GetAllForGroupUserAsync(Guid groupId);
        Task<IEnumerable<List>> GetAllForGroupTeamAsync(Guid groupId);
    }
}
