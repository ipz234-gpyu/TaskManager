using Dapper;
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

        public async Task<IEnumerable<GroupFromUser>> GetAllForUserAsync(Guid userId)
        {
            using var connection = _connectionFactory.Create();
            var sql = $"SELECT * FROM {_tableName} WHERE UserId = @UserId";

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            return await connection.QueryAsync<GroupFromUser>(sql, parameters);
        }
    }
}
