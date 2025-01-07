using System.Data;

namespace TaskManager.Server.Infrastructure.DataBase;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}
