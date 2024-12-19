using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.Infrastructure.Repositories
{
    public class GroupsFromUserRepository : Repository<GroupFromUser>, IGroupsFromUserRepository
    {
        public GroupsFromUserRepository(ISqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
    }
}
