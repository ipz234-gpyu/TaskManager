using Dapper;
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

        public async virtual Task<UserRefreshToken> GetAsyncByUserIdDeviceInfo(Guid userId, string deviceInfo)
        {
            using var connection = _connectionFactory.Create();
            const string sql = @"
                SELECT [TokenId], [UserId], [RefreshTokenHash], [ExpiresAt], [DeviceInfo]
                FROM [UserRefreshTokens]
                WHERE [UserId] = @UserId AND [DeviceInfo] = @DeviceInfo";

            return await connection.QuerySingleOrDefaultAsync<UserRefreshToken>(sql,
                 new { UserId = userId, DeviceInfo = deviceInfo });
        }

        public override async Task<UserRefreshToken> CreateAsync(UserRefreshToken entity)
        {
            using var connection = _connectionFactory.Create();
            const string sql = @"
                INSERT UserRefreshTokens (UserId, RefreshTokenHash, ExpiresAt, DeviceInfo)
                VALUES (@UserId, @RefreshTokenHash, @ExpiresAt, @DeviceInfo)
                ";

            var parameterValues = new DynamicParameters();
            parameterValues.Add("@UserId", entity.UserId);
            parameterValues.Add("@RefreshTokenHash", entity.RefreshTokenHash);
            parameterValues.Add("@ExpiresAt", entity.ExpiresAt);
            parameterValues.Add("@DeviceInfo", entity.DeviceInfo);

            await connection.ExecuteAsync(sql, parameterValues);
            return await GetAsyncByUserIdDeviceInfo(entity.UserId, entity.DeviceInfo);
        }
    }
}
