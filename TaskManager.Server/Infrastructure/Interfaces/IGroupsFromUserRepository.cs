using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.Infrastructure.Interfaces;

public interface IGroupsFromUserRepository : IRepository<GroupFromUser>
{
    Task<IEnumerable<GroupFromUser>> GetAllForUserAsync(Guid UserId);
}