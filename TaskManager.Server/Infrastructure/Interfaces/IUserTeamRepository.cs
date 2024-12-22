using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.Infrastructure.Interfaces
{
    public interface IUserTeamRepository : IRepository<UserTeam>
    {
        Task<UserTeam> GetByUserIdAndTeamId(Guid TeamId, Guid UserId);
    }
}
