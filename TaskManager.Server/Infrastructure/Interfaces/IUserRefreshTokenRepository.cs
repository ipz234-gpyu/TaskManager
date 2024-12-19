using TaskManager.Server.Domain.Entities;

namespace TaskManager.Server.Infrastructure.Interfaces
{
    public interface IUserRefreshTokenRepository : IRepository<UserRefreshToken>
    {
        Task<UserRefreshToken> GetAsyncByUserIdDeviceInfo(Guid UserId, string deviceInfo);
    }
}
