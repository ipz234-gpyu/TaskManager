using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.Infrastructure.Interfaces;

public interface ITeamInvitationRepository : IRepository<TeamInvitation>
{
    Task<TeamInvitation> GetByUserIdAndTeamId(Guid TeamId, Guid UserId);
}
