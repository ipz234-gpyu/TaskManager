using Dapper;
using System.Text.RegularExpressions;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.Infrastructure.Repositories
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        public TeamRepository(ISqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async virtual Task<IEnumerable<Team>> GetAllByUserId(Guid userId)
        {
            using var connection = _connectionFactory.Create();

            var sql = @"
                        SELECT T.*
                        FROM Teams T
                        INNER JOIN UserTeams UT ON T.TeamId = UT.TeamId
                        WHERE UT.UserId = @UserId
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            return await connection.QueryAsync<Team>(sql, parameters);
        }
    }
}
