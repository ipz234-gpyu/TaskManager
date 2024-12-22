using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.Infrastructure.Interfaces
{
    public interface ITeamRepository : IRepository<Team>
    {
        Task<IEnumerable<Team>> GetAllByUserId(Guid userId);
    }
}
