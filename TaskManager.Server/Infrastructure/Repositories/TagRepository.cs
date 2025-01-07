using Dapper;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Server.Infrastructure.Repositories;

public class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(ISqlConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public virtual async Task<IEnumerable<Tag>> GetAllForUser(Guid userId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"
                        SELECT *
                        FROM Tags
                        WHERE UserId = @UserId OR UserId IS NULL;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);

        return await connection.QueryAsync<Tag>(sql, parameters);
    }

    public virtual async Task<IEnumerable<Tag>> GetAllForTeam(Guid teamId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"
                SELECT DISTINCT Tags.*
                FROM Tags
                LEFT JOIN Users ON Users.UserId = Tags.UserId
                LEFT JOIN UserTeams ON UserTeams.UserId = Users.UserId
                WHERE UserTeams.TeamId = @TeamId OR Tags.UserId IS NULL;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@TeamId", teamId);

        return await connection.QueryAsync<Tag>(sql, parameters);
    }

    public virtual async Task CreateConnection(Guid tagId, Guid taskId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"     
                INSERT TaskTag (TaskId, TagId)
                VALUES (@TaskId, @TagId)
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@TaskId", taskId);
        parameters.Add("@TagId", tagId);

        await connection.ExecuteAsync(sql, parameters);
    }

    public virtual async Task DeleteConnection(Guid tagId, Guid taskId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"     
                DELETE FROM TaskTag
                WHERE TaskId = @TaskId AND TagId = @TagId;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@TaskId", taskId);
        parameters.Add("@TagId", tagId);

        await connection.ExecuteAsync(sql, parameters);
    }

    public virtual async Task<IEnumerable<Guid>> GetTagIdsByTaskId(Guid taskId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"SELECT Tags.TagId
                  FROM Tags
                  INNER JOIN TaskTag ON Tags.TagId = TaskTag.TagId
                  WHERE TaskTag.TaskId = @TaskId";

        var parameters = new DynamicParameters();
        parameters.Add("@TaskId", taskId);

        return await connection.QueryAsync<Guid>(sql, parameters);
    }
}
