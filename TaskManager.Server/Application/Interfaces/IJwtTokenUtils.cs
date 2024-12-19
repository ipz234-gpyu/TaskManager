using System.Security.Claims;
using TaskManager.Server.Application.Models;

namespace TaskManager.Server.Application.Interfaces
{
    public interface IJwtTokenUtils
    {
        TokenResponse GenerateToken(Claim[] claims, DateTime expire);
        ClaimsPrincipal? ValidateToken(string token);
        TokenResponse GenerateAccessToken(Guid UserId);
        Task<TokenResponse> GenerateRefreshToken(Guid UserId, string deviceInfo);
        Task RevokeRefreshToken(Guid TokenId, string tokenHash);
    }
}
