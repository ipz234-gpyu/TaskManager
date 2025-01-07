using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.Infrastructure.Interfaces;

public interface IGroupsFromTeamRepository : IRepository<GroupFromTeam>
{
    Task<IEnumerable<GroupFromTeam>> GetAllForTeamAsync(Guid TeamId);
}