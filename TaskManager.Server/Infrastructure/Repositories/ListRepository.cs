using Dapper;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace TaskManager.Server.Infrastructure.Repositories;

public class ListRepository : Repository<List>, IListRepository
{
    public ListRepository(ISqlConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async virtual Task<List> CreateForUserAsync(Guid groupId, List entity)
    {
        using var connection = _connectionFactory.Create();

        var sql = $"EXEC CreateListForUser @GroupId, @ListName, @Priority;";

        var parameterValues = new DynamicParameters();
        parameterValues.Add("@GroupId", groupId);
        parameterValues.Add("@ListName", entity.Name);
        parameterValues.Add("@Priority", entity.Priority);

        return await connection.QuerySingleAsync<List>(sql, parameterValues);
    }

    public async virtual Task<List> CreateForTeamAsync(Guid groupId, List entity)
    {
        using var connection = _connectionFactory.Create();

        var sql = $"EXEC CreateListForTeam @GroupId, @ListName, @Priority;";

        var parameterValues = new DynamicParameters();
        parameterValues.Add("@GroupId", groupId);
        parameterValues.Add("@ListName", entity.Name);
        parameterValues.Add("@Priority", entity.Priority);

        return await connection.QuerySingleAsync<List>(sql, parameterValues);
    }

    public async virtual Task<IEnumerable<List>> GetAllForGroupUserAsync(Guid groupId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"
                        SELECT L.*
                        FROM Lists L
                        INNER JOIN GroupListsFromUser GLFU ON L.ListId = GLFU.ListId
                        WHERE GLFU.GroupsFromUserId = @GroupId
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@GroupId", groupId);

        return await connection.QueryAsync<List>(sql, parameters);
    }

    public async virtual Task<IEnumerable<List>> GetAllForGroupTeamAsync(Guid groupId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"
                        SELECT L.*
                        FROM Lists L
                        INNER JOIN GroupListsFromTeam GLFU ON L.ListId = GLFU.ListId
                        WHERE GLFU.GroupsFromTeamId = @GroupId
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@GroupId", groupId);

        return await connection.QueryAsync<List>(sql, parameters);
    }

    public async virtual Task CreateConnectionForUser(Guid groupId, Guid listId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"     
                INSERT GroupListsFromUser (GroupsFromUserId, ListId)
                VALUES (@GroupId, @ListId)
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@GroupId", groupId);
        parameters.Add("@ListId", listId);

        await connection.ExecuteAsync(sql, parameters);
    }

    public async virtual Task CreateConnectionForTeam(Guid groupId, Guid listId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"     
                INSERT GroupListsFromTeam (GroupsFromTeamId, ListId)
                VALUES (@GroupId, @ListId)
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@GroupId", groupId);
        parameters.Add("@ListId", listId);

        await connection.ExecuteAsync(sql, parameters);
    }

    public async virtual Task DeleteConnectionForUser(Guid groupId, Guid listId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"     
                DELETE FROM GroupListsFromUser
                WHERE GroupsFromUserId = @GroupId AND ListId = @ListId;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@GroupId", groupId);
        parameters.Add("@ListId", listId);

        await connection.ExecuteAsync(sql, parameters);
    }

    public async virtual Task DeleteConnectionForTeam(Guid groupId, Guid listId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"     
                DELETE FROM GroupListsFromTeam
                WHERE GroupsFromTeamId = @GroupId AND ListId = @ListId;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@GroupId", groupId);
        parameters.Add("@ListId", listId);

        await connection.ExecuteAsync(sql, parameters);
    }
}
