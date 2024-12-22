using Dapper;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Server.Infrastructure.Repositories
{
    public class UserTeamRepository : Repository<UserTeam>, IUserTeamRepository
    {
        public UserTeamRepository(ISqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public virtual async Task<UserTeam?> GetByUserIdAndTeamId(Guid teamId, Guid userId)
        {
            using var connection = _connectionFactory.Create();
            const string sql = @"
            SELECT *
            FROM UserTeams
            WHERE TeamId = @TeamId
            AND UserId = @UserId
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@TeamId", teamId);
            parameters.Add("@UserId", userId);

            return await connection.QuerySingleOrDefaultAsync<UserTeam>(sql, parameters);
        }

        public virtual async Task DeleteByUserIdAndTeamId(Guid teamId, Guid userId)
        {
            using var connection = _connectionFactory.Create();
            const string sql = @"
            DELETE FROM UserTeams
            WHERE TeamId = @TeamId
            AND UserId = @UserId
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@TeamId", teamId);
            parameters.Add("@UserId", userId);

            await connection.ExecuteAsync(sql, parameters);
        }

    }
}
