using System.Security.Claims;
using TaskManager.Server.Application.Models;

namespace TaskManager.Server.Application.Interfaces;

public interface IJwtTokenUtils
{
    TokenResponse GenerateToken(Claim[] claims, DateTime expire);
    ClaimsPrincipal? ValidateToken(string token);
    TokenResponse GenerateAccessToken(Guid UserId);
    Task<TokenResponse> GenerateRefreshToken(Guid UserId, string deviceInfo);
    Task<TokenResponse> GenerateInviteToken(Guid TeamId, Guid UserId);
    Task RevokeRefreshToken(Guid TokenId, string tokenHash);
    Task ValidateInviteToken(string Token, out Guid TeamId, out Guid UserId);
}
