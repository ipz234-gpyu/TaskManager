using Dapper;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.Infrastructure.Repositories;

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

    public virtual async Task<IEnumerable<TaskAssignment>> GetAllTaskAssignments(Guid teamId)
    {
        using var connection = _connectionFactory.Create();

        var sql = @"
                         SELECT ut.UserId AS UserTeamId, ta.TaskId
                         FROM TaskAssignments ta
                         INNER JOIN UserTeams ut ON ta.UserTeamId = ut.UserTeamId
                         WHERE ut.TeamId = @TeamId;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@TeamId", teamId);

        return await connection.QueryAsync<TaskAssignment>(sql, parameters);
    }
}
