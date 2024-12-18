using Dapper;
using TaskManager.Server.Infrastructure.Attributes;
using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.Infrastructure.Repositories;

public class Repository<T> : IRepository<T>
{
    protected readonly ISqlConnectionFactory _connectionFactory;
    protected readonly string _tableName;
    protected readonly string _columnIdName;
    public Repository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;

        var tableNameAttribute = (TableNameAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(TableNameAttribute));
        if (tableNameAttribute == null)
            throw new InvalidOperationException($"TableNameAttribute is not defined for type {typeof(T).Name}.");

        _tableName = tableNameAttribute.TableName;

        var columnIdName = (ColumnIdNameAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(ColumnIdNameAttribute));
        if (columnIdName == null)
            throw new InvalidOperationException($"ColumnIdNameAttribute is not defined for type {typeof(T).Name}.");
        _columnIdName = columnIdName.ColumnIdName;
    }
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        using var connection = _connectionFactory.Create();
        var sql = $"SELECT * FROM {_tableName}";
        return await connection.QueryAsync<T>(sql);
    }
    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.Create();
        var sql = $"SELECT * FROM {_tableName} WHERE {_columnIdName} = @{_columnIdName}";

        var parameters = new DynamicParameters();
        parameters.Add($"@{_columnIdName}", id);

        var result = await connection.QuerySingleOrDefaultAsync<T>(sql, parameters);
        return result;
    }
    public virtual async Task<T> CreateAsync(T entity)
    {
        using var connection = _connectionFactory.Create();

        var entityType = typeof(T);
        var properties = entityType.GetProperties()
            .Where(p => p.Name != _columnIdName);

        var columns = string.Join(", ", properties.Select(p => p.Name));
        var parameters = string.Join(", ", properties.Select(p => "@" + p.Name));

        var sql = $"INSERT INTO {_tableName} ({columns}) OUTPUT INSERTED.* VALUES ({parameters})";

        var parameterValues = new DynamicParameters();
        foreach (var property in properties)
            parameterValues.Add("@" + property.Name, property.GetValue(entity));

        var insertedEntity = await connection.QuerySingleAsync<T>(sql, parameterValues);
        return insertedEntity;
    }
    public virtual async Task<T> UpdateAsync(T entity)
    {
        var entityType = typeof(T);

        using var connection = _connectionFactory.Create();
        var properties = entityType.GetProperties()
                                   .Where(p => p.Name != _columnIdName)
                                   .Select(p => $"{p.Name} = @{p.Name}");

        var setClause = string.Join(", ", properties);
        var sql = $"UPDATE {_tableName} SET {setClause} WHERE {_columnIdName} = @{_columnIdName}";

        await connection.ExecuteAsync(sql, entity);

        var sqlSelect = $"SELECT * FROM {_tableName} WHERE {_columnIdName} = @{_columnIdName}";
        var parameters = new DynamicParameters();
        parameters.Add($"@{_columnIdName}", entityType.GetProperty(_columnIdName).GetValue(entity));
        return await connection.QuerySingleOrDefaultAsync<T>(sqlSelect, parameters);
    }
    public virtual async Task DeleteAsync(Guid id)
    {
        using var connection = _connectionFactory.Create();
        var sql = $"DELETE FROM {_tableName} WHERE {_columnIdName} = @{_columnIdName}";
        var parameters = new DynamicParameters();
        parameters.Add($"@{_columnIdName}", id);
        await connection.ExecuteAsync(sql, parameters);
    }
}