using Dapper;
using GraphQL.Types.Relay.DataObjects;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.Infrastructure.Repositories
{
    public class UserRefreshTokenRepository : Repository<UserRefreshToken>, IUserRefreshTokenRepository
    {
        public UserRefreshTokenRepository(ISqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public async Task<UserRefreshToken> GetAsyncByUserIdDeviceInfo(Guid userId, string deviceInfo)
        {
            using var connection = _connectionFactory.Create();
            const string sql = @"
                SELECT [TokenId], [UserId], [RefreshTokenHash], [ExpiresAt], [DeviceInfo]
                FROM [UserRefreshTokens]
                WHERE [UserId] = @UserId AND [DeviceInfo] = @DeviceInfo";

           return await connection.QuerySingleOrDefaultAsync<UserRefreshToken>(sql,
                new { UserId = userId, DeviceInfo = deviceInfo });
        }
    }
}
