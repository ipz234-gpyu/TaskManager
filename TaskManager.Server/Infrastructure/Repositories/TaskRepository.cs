using Dapper;
using System.Data;
using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;
using static Dapper.SqlMapper;
using Task = TaskManager.Server.Domain.Entities.Task;

namespace TaskManager.Server.Infrastructure.Repositories;

public class TaskRepository : Repository<Task>, ITaskRepository
{
    public TaskRepository(ISqlConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public override async System.Threading.Tasks.Task DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"
	            EXEC DeleteTaskAndChildren @TaskId;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@TaskId", id);

        await connection.ExecuteAsync(sql, parameters);
    }

    public override async Task<Task> CreateAsync(Task entity)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"     
                DECLARE @TaskId UNIQUEIDENTIFIER = NEWID();
                
                INSERT INTO Tasks (TaskId, Title, Description, StartTime, Deadline, Notification, Priority, Status, ParentTaskId)
	            VALUES (@TaskId, @Title, @Description, @StartTime, @Deadline, @Notification, @Priority, @Status, @ParentTaskId);

                SELECT * 
                FROM Tasks 
                WHERE Id = @TaskId;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@Title", entity.Title);
        parameters.Add("@Description", entity.Description);
        parameters.Add("@StartTime", entity.StartTime);
        parameters.Add("@Deadline", entity.Deadline);
        parameters.Add("@Notification", entity.Notification);
        parameters.Add("@Priority", entity.Priority);
        parameters.Add("@Status", entity.Status);
        parameters.Add("@ParentTaskId", entity.ParentTaskId);

        return await connection.QuerySingleAsync<Task>(sql, parameters);
    }

    public virtual async Task<Task> CreateForList(Task entity, Guid listId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"
	            EXEC CreateTask @ListId, @Title, @Description, @StartTime, @Deadline, @Notification, @Priority, @Status, @ParentTaskId;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@ListId", listId);
        parameters.Add("@Title", entity.Title);
        parameters.Add("@Title", entity.Title);
        parameters.Add("@Description", entity.Description);
        parameters.Add("@StartTime", entity.StartTime);
        parameters.Add("@Deadline", entity.Deadline);
        parameters.Add("@Notification", entity.Notification);
        parameters.Add("@Priority", entity.Priority);
        parameters.Add("@Status", entity.Status);
        parameters.Add("@ParentTaskId", entity.ParentTaskId);

        return await connection.QuerySingleAsync<Task>(sql, parameters);
    }

    public virtual async Task<IEnumerable<Task>> GetAllForList(Guid listId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"
                        SELECT T.*
                        FROM Tasks T
                        INNER JOIN TaskLists ON T.TaskId = TaskLists.TaskId
                        WHERE TaskLists.ListId = @ListId
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@ListId", listId);

        return await connection.QueryAsync<Task>(sql, parameters);
    }

    public virtual async System.Threading.Tasks.Task CreateConnection(Guid listId, Guid taskId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"     
                INSERT TaskLists (TaskId, ListId)
                VALUES (@TaskId, @ListId)
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@TaskId", taskId);
        parameters.Add("@ListId", listId);

        await connection.ExecuteAsync(sql, parameters);
    }

    public virtual async System.Threading.Tasks.Task DeleteConnection(Guid listId, Guid taskId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"     
                DELETE FROM TaskLists
                WHERE TaskId = @TaskId AND ListId = @ListId;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@TaskId", taskId);
        parameters.Add("@ListId", listId);

        await connection.ExecuteAsync(sql, parameters);
    }

    public virtual async Task<IEnumerable<Task>> GetUserTasksInDateRange(Guid userId, DateTime startTime, DateTime endTime)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"     
                EXEC GetUserTasksInDateRange 
                    @UserId, 
                    @StartTime, 
                    @EndTime;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        parameters.Add("@StartTime", startTime);
        parameters.Add("@EndTime", endTime);

        return await connection.QueryAsync<Task>(sql, parameters);
    }

    public virtual async Task<IEnumerable<Task>> GetTeamTasksInDateRange(Guid teamId, DateTime startTime, DateTime endTime)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"     
                EXEC GetUserTasksInDateRange 
                    @TeamId, 
                    @StartTime, 
                    @EndTime;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@TeamId", teamId);
        parameters.Add("@StartTime", startTime);
        parameters.Add("@EndTime", endTime);

        return await connection.QueryAsync<Task>(sql, parameters);
    }

    public virtual async System.Threading.Tasks.Task AssignTaskToTeamMember(Guid userId, Guid teamId, Guid taskId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"     
                EXEC AssignTaskToTeamMember 
                    @UserId, 
                    @TeamId, 
                    @TaskId;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        parameters.Add("@TeamId", teamId);
        parameters.Add("@TaskId", taskId);

        await connection.QueryAsync<Task>(sql, parameters);
    }

    public virtual async System.Threading.Tasks.Task RemoveTaskFromTeamMember(Guid userId, Guid teamId, Guid taskId)
    {
        using var connection = _connectionFactory.Create();
        var sql = @"     
                EXEC RemoveTaskFromTeamMember 
                    @UserId, 
                    @TeamId, 
                    @TaskId;
            ";

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        parameters.Add("@TeamId", teamId);
        parameters.Add("@TaskId", taskId);

        await connection.QueryAsync<Task>(sql, parameters);
    }

    public virtual async Task<IEnumerable<Task>> GetUserTasksByTags(Guid userId, IEnumerable<Guid> tagIds)
    {
        using var connection = _connectionFactory.Create();
        var sql = "EXEC GetUserTasksByTags @UserId, @TagIds";

        var tagIdsDataTable = new DataTable();
        tagIdsDataTable.Columns.Add("TagId", typeof(Guid));
        foreach (var tagId in tagIds)
            tagIdsDataTable.Rows.Add(tagId);

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId);
        parameters.Add("@TagIds", tagIdsDataTable.AsTableValuedParameter("TagIdsType"));

        var tasks = await connection.QueryAsync<Task>(sql, parameters);
        return tasks;
    }
    public virtual async Task<IEnumerable<Task>> GetTeamTasksByTags(Guid teamId, IEnumerable<Guid> tagIds)
    {
        using var connection = _connectionFactory.Create();
        var sql = "EXEC GetTeamTasksByTags @TeamId, @TagIds";

        var tagIdsDataTable = new DataTable();
        tagIdsDataTable.Columns.Add("TagId", typeof(Guid));
        foreach (var tagId in tagIds)
            tagIdsDataTable.Rows.Add(tagId);

        var parameters = new DynamicParameters();
        parameters.Add("@TeamId", teamId);
        parameters.Add("@TagIds", tagIdsDataTable.AsTableValuedParameter("TagIdsType"));

        var tasks = await connection.QueryAsync<Task>(sql, parameters);
        return tasks;
    }

}