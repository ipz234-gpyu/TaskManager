using Dapper;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.Infrastructure.Repositories
{
    public class GroupsFromTeamRepository : Repository<GroupFromTeam>, IGroupsFromTeamRepository
    {
        public GroupsFromTeamRepository(ISqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<IEnumerable<GroupFromTeam>> GetAllForTeamAsync(Guid teamId)
        {
            using var connection = _connectionFactory.Create();
            var sql = $"SELECT * FROM {_tableName} WHERE TeamId = @TeamId";

            var parameters = new DynamicParameters();
            parameters.Add("@TeamId", teamId);

            return await connection.QueryAsync<GroupFromTeam>(sql, parameters);
        }
    }
}
