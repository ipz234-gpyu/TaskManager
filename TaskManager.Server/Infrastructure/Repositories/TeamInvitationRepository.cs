using Dapper;
using TaskManager.Server.Domain.Entities;
using TaskManager.Server.Infrastructure.DataBase;
using TaskManager.Server.Infrastructure.Interfaces;

namespace TaskManager.Server.Infrastructure.Repositories
{
    public class TeamInvitationRepository : Repository<TeamInvitation>, ITeamInvitationRepository
    {
        public TeamInvitationRepository(ISqlConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public virtual async Task<TeamInvitation?> GetByUserIdAndTeamId(Guid teamId, Guid userId)
        {
            using var connection = _connectionFactory.Create();
            const string sql = @"
            SELECT * 
            FROM TeamInvitations
            WHERE TeamId = @TeamId
            AND UserId = @UserId
            ";

            var parameters = new DynamicParameters();
            parameters.Add("@TeamId", teamId);
            parameters.Add("@UserId", userId);

            return await connection.QuerySingleOrDefaultAsync<TeamInvitation>(sql, parameters);
        }

        public override async Task<TeamInvitation> CreateAsync(TeamInvitation entity)
        {
            using var connection = _connectionFactory.Create();
            const string sql = @"
                INSERT TeamInvitations (TokenHash, ExpiresAt, UserId, TeamId)
                VALUES (@TokenHash, @ExpiresAt, @UserId, @TeamId)
                ";

            var parameterValues = new DynamicParameters();
            parameterValues.Add("@TokenHash", entity.TokenHash);
            parameterValues.Add("@ExpiresAt", entity.ExpiresAt);
            parameterValues.Add("@UserId", entity.UserId);
            parameterValues.Add("@TeamId", entity.TeamId);

            await connection.ExecuteAsync(sql, parameterValues);
            return await GetByUserIdAndTeamId(entity.TeamId, entity.UserId);
        }
    }
}
