using TaskManager.Server.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Server.Infrastructure.Interfaces;

public interface IUserTeamRepository : IRepository<UserTeam>
{
    Task<UserTeam> GetByUserIdAndTeamId(Guid TeamId, Guid UserId);
    Task DeleteByUserIdAndTeamId(Guid teamId, Guid userId);
}
